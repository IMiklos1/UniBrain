using Microsoft.EntityFrameworkCore;
using UniBrain.Data;
using UniBrain.Models;

namespace UniBrain
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite("Data Source=uni_brain.db"));

            // Add Swagger/OpenAPI
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // CORS engedélyezése (fejlesztés alatt hasznos, ha külön fut a React)
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowReactDev",
                    builder => builder.WithOrigins("http://localhost:5173", "http://localhost:5000") // Vite alapértelmezett port
                                      .AllowAnyMethod()
                                      .AllowAnyHeader());
            });

            var app = builder.Build();

            // Configure Swagger UI
            app.UseSwagger();
            app.UseSwaggerUI();

            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                db.Database.EnsureCreated(); // Létrehozza az adatbázist, ha nincs
                DbSeeder.Seed(db);           // Feltölti az órarenddel
            }

            var uploadsPath = Path.Combine(app.Environment.WebRootPath ?? "wwwroot", "uploads");
            Directory.CreateDirectory(uploadsPath);

            app.UseCors("AllowReactDev");

            app.UseDefaultFiles(); // index.html keresése
            app.UseStaticFiles();  // wwwroot mappa kiszolgálása
            app.MapFallbackToFile("index.html"); // Minden más útvonalat a React kezel

            app.MapGet("/api/schedule", async (AppDbContext db) =>
            {
                var sessions = await db.Sessions
                    .Include(s => s.Subject) // Join
                    .ToListAsync();

                // Átalakítás olyan formátumra, amit a Frontend naptár szeret
                return sessions.Select(s => new
                {
                    id = s.Id,
                    title = s.Subject.Name + (s.Type == "potlo" ? " (Pótlás)" : ""),
                    start = s.StartTime,
                    end = s.EndTime,
                    extendedProps = new
                    {
                        room = s.Room,
                        teacher = s.Subject.Teacher,
                        subjectId = s.Subject.Id
                    },
                    // Színkódolás típus alapján
                    backgroundColor = s.Type == "potlo" ? "#d32f2f" : "#1976d2"
                });
            });

            app.MapGet("/api/subjects", async (AppDbContext db) =>
                await db.Subjects.Include(s => s.Sessions).Include(s => s.Notes).ToListAsync());

            app.MapPost("/api/notes", async (AppDbContext db, Note note) =>
            {
                // Megnézzük, van-e érvényes kapcsolódása
                if (note.ClassSessionId == null && note.SubjectId == null)
                    return Results.BadRequest("A jegyzetnek kapcsolódnia kell órához vagy tárgyhoz.");

                note.CreatedAt = DateTime.Now;
                db.Notes.Add(note);
                await db.SaveChangesAsync();
                return Results.Created($"/api/notes/{note.Id}", note);
            });

            app.MapGet("/api/notes/session/{sessionId}", async (AppDbContext db, int sessionId) =>
            {
                return await db.Notes
                    .Where(n => n.ClassSessionId == sessionId)
                    .OrderByDescending(n => n.CreatedAt)
                    .ToListAsync();
            });

            app.MapDelete("/api/notes/{id}", async (AppDbContext db, int id) =>
            {
                var note = await db.Notes.FindAsync(id);
                if (note == null)
                    return Results.NotFound();
                db.Notes.Remove(note);
                await db.SaveChangesAsync();
                return Results.Ok();
            });

            app.MapPost("/api/attachments", async (HttpRequest request, AppDbContext db) =>
            {
                // Ellenőrizzük, hogy multipart/form-data kérés-e
                if (!request.HasFormContentType)
                    return Results.BadRequest("Csak fájlfeltöltés engedélyezett.");

                var form = await request.ReadFormAsync();
                var file = form.Files.GetFile("file"); // A frontend 'file' néven küldi
                var sessionIdStr = form["classSessionId"]; // És küldi az ID-t is

                if (file == null || file.Length == 0 || string.IsNullOrEmpty(sessionIdStr))
                    return Results.BadRequest("Hiányzó fájl vagy óra ID.");

                int sessionId = int.Parse(sessionIdStr!);

                // Fájl mentése a lemezre (egyedi névvel, hogy ne írják felül egymást)
                var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
                var savePath = Path.Combine("wwwroot", "uploads", uniqueFileName);

                using (var stream = new FileStream(savePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Adatbázis bejegyzés
                var attachment = new Attachment
                {
                    FileName = file.FileName,
                    StoredFileName = uniqueFileName,
                    ContentType = file.ContentType,
                    ClassSessionId = sessionId
                };

                db.Attachments.Add(attachment);
                await db.SaveChangesAsync();

                return Results.Ok(attachment);
            });

            app.MapGet("/api/attachments/session/{sessionId}", async (AppDbContext db, int sessionId) =>
            {
                return await db.Attachments
                    .Where(a => a.ClassSessionId == sessionId)
                    .ToListAsync();
            });

            app.MapDelete("/api/attachments/{id}", async (AppDbContext db, int id) =>
            {
                var attachment = await db.Attachments.FindAsync(id);
                if (attachment == null) return Results.NotFound();

                // 1. Törlés a lemezről
                var filePath = Path.Combine("wwwroot", "uploads", attachment.StoredFileName);
                if (File.Exists(filePath)) File.Delete(filePath);

                // 2. Törlés a DB-ből
                db.Attachments.Remove(attachment);
                await db.SaveChangesAsync();

                return Results.Ok();
            });

            app.Run();
        }
    }
}
