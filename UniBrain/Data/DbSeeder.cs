// IMPORT DATA FROM JSON TO DB
using System.Text.Json;
using UniBrain.Models;

namespace UniBrain.Data
{

    public static class DbSeeder
    {
        // Ez a struktúra csak a JSON beolvasásához kell
        private class RawScheduleItem
        {
            public string date { get; set; }
            public string start { get; set; }
            public string end { get; set; }
            public string subject { get; set; }
            public string code { get; set; }
            public string room { get; set; }
            public string teacher { get; set; }
            public string type { get; set; }
        }

        public static void Seed(AppDbContext context)
        {
            // Ha már van adat, nem csinálunk semmit
            if (context.Subjects.Any()) return;

            // Itt a JSON sztringed (vagy beolvashatod fájlból is)
            string json = @"
            [
  { ""date"": ""2026-02-13"", ""day"": ""P"", ""start"": ""16:00"", ""end"": ""19:20"", ""subject"": ""Az információtechnika fizikai alap"", ""code"": ""GEFIT006-ML"", ""room"": ""A1/307"", ""teacher"": ""Dr. Kovács Endre"", ""type"": ""normal"" },

  { ""date"": ""2026-02-20"", ""day"": ""P"", ""start"": ""12:30"", ""end"": ""15:50"", ""subject"": ""Beágyazott rendszerek"", ""code"": ""GEVAU160-ML"", ""room"": ""In/214"", ""teacher"": ""Dr. Vásárhelyi József"", ""type"": ""normal"" },
  { ""date"": ""2026-02-20"", ""day"": ""P"", ""start"": ""16:00"", ""end"": ""19:20"", ""subject"": ""Irányítási rendszerek tervezése"", ""code"": ""GEVAU120-ML"", ""room"": ""In/1"", ""teacher"": ""Forgács Zsófia"", ""type"": ""normal"" },
  { ""date"": ""2026-02-21"", ""day"": ""So"", ""start"": ""08:30"", ""end"": ""11:50"", ""subject"": ""Az információtechnika fizikai alap"", ""code"": ""GEFIT006-ML"", ""room"": ""A1/307"", ""teacher"": ""Dr. Kovács Endre"", ""type"": ""normal"" },
  { ""date"": ""2026-02-21"", ""day"": ""So"", ""start"": ""13:20"", ""end"": ""16:40"", ""subject"": ""Automatika"", ""code"": ""GEVAU502-BL2"", ""room"": ""XXXII"", ""teacher"": ""Móré Ádám"", ""type"": ""potlo"" },

  { ""date"": ""2026-02-27"", ""day"": ""P"", ""start"": ""16:00"", ""end"": ""19:20"", ""subject"": ""Irányítási rendszerek tervezése"", ""code"": ""GEVAU120-ML"", ""room"": ""In/214"", ""teacher"": ""Simon Róbert"", ""type"": ""normal"" },
  { ""date"": ""2026-02-27"", ""day"": ""P"", ""start"": ""16:00"", ""end"": ""19:20"", ""subject"": ""Villamos energetika és bizt. tech."", ""code"": ""GEVEE511-BL2"", ""room"": ""A1/320"", ""teacher"": ""Dr. Kozsely Gábor"", ""type"": ""potlo"" },
  { ""date"": ""2026-02-28"", ""day"": ""So"", ""start"": ""13:20"", ""end"": ""16:40"", ""subject"": ""Automatika"", ""code"": ""GEVAU502-BL2"", ""room"": ""XXXII"", ""teacher"": ""Móré Ádám"", ""type"": ""potlo"" },
  { ""date"": ""2026-02-28"", ""day"": ""So"", ""start"": ""08:30"", ""end"": ""11:50"", ""subject"": ""Az információtechnika fizikai alap"", ""code"": ""GEFIT006-ML"", ""room"": ""A1/307"", ""teacher"": ""Dr. Kovács Endre"", ""type"": ""normal"" },

  { ""date"": ""2026-03-06"", ""day"": ""P"", ""start"": ""12:30"", ""end"": ""15:50"", ""subject"": ""Irányítási rendszerek tervezése"", ""code"": ""GEVAU120-ML"", ""room"": ""In/214"", ""teacher"": ""Simon Róbert"", ""type"": ""normal"" },
  { ""date"": ""2026-03-06"", ""day"": ""P"", ""start"": ""16:00"", ""end"": ""19:20"", ""subject"": ""Automaták és formális nyelvek"", ""code"": ""GEMAN385-ML"", ""room"": ""A1/12"", ""teacher"": ""Dr. Veres Laura"", ""type"": ""normal"" },
  { ""date"": ""2026-03-07"", ""day"": ""So"", ""start"": ""08:30"", ""end"": ""11:50"", ""subject"": ""Az információtechnika fizikai alap"", ""code"": ""GEFIT006-ML"", ""room"": ""A1/307"", ""teacher"": ""Dr. Kovács Endre"", ""type"": ""normal"" },
  { ""date"": ""2026-03-07"", ""day"": ""So"", ""start"": ""12:20"", ""end"": ""15:40"", ""subject"": ""Automatika"", ""code"": ""GEVAU502-BL2"", ""room"": ""XXXII"", ""teacher"": ""Móré Ádám"", ""type"": ""potlo"" },

  { ""date"": ""2026-03-14"", ""day"": ""So"", ""start"": ""08:30"", ""end"": ""11:50"", ""subject"": ""Szv. A munka jövője- jogi& t.vált."", ""code"": ""AJAMU09GELM"", ""room"": ""I"", ""teacher"": ""Dr. Mélypataki Gábor"", ""type"": ""normal"" },
  { ""date"": ""2026-03-14"", ""day"": ""So"", ""start"": ""08:30"", ""end"": ""11:50"", ""subject"": ""Villamos energetika és bizt. tech."", ""code"": ""GEVEE511-BL2"", ""room"": ""A1/320"", ""teacher"": ""Dr. Kozsely Gábor"", ""type"": ""potlo"" },

  { ""date"": ""2026-03-20"", ""day"": ""P"", ""start"": ""12:30"", ""end"": ""15:50"", ""subject"": ""Differenciálegyenletek"", ""code"": ""GEMAN500-ML"", ""room"": ""A1/320"", ""teacher"": ""Dr. Varga Péter - GEK-es"", ""type"": ""normal"" },
  { ""date"": ""2026-03-20"", ""day"": ""P"", ""start"": ""16:00"", ""end"": ""19:20"", ""subject"": ""Beágyazott rendszerek"", ""code"": ""GEVAU160-ML"", ""room"": ""In/214"", ""teacher"": ""Dr. Vásárhelyi József"", ""type"": ""normal"" },
  { ""date"": ""2026-03-21"", ""day"": ""So"", ""start"": ""08:30"", ""end"": ""11:50"", ""subject"": ""Szv. A munka jövője- jogi& t.vált."", ""code"": ""AJAMU09GELM"", ""room"": ""XX"", ""teacher"": ""Dr. Mélypataki Gábor"", ""type"": ""normal"" },

  { ""date"": ""2026-03-27"", ""day"": ""P"", ""start"": ""16:00"", ""end"": ""19:20"", ""subject"": ""Automaták és formális nyelvek"", ""code"": ""GEMAN385-ML"", ""room"": ""OnLine!"", ""teacher"": ""Dr. Veres Laura"", ""type"": ""normal"" },
  { ""date"": ""2026-03-28"", ""day"": ""So"", ""start"": ""08:30"", ""end"": ""11:50"", ""subject"": ""Jelek és rendszerek elmélete"", ""code"": ""GEVAU220-ML"", ""room"": ""In/201"", ""teacher"": ""Móré Ádám"", ""type"": ""normal"" },

  { ""date"": ""2026-04-17"", ""day"": ""P"", ""start"": ""12:30"", ""end"": ""15:50"", ""subject"": ""Irányítási rendszerek tervezése"", ""code"": ""GEVAU120-ML"", ""room"": ""In/1"", ""teacher"": ""Forgács Zsófia"", ""type"": ""normal"" },
  { ""date"": ""2026-04-17"", ""day"": ""P"", ""start"": ""16:00"", ""end"": ""19:20"", ""subject"": ""Differenciálegyenletek"", ""code"": ""GEMAN500-ML"", ""room"": ""A1/320"", ""teacher"": ""Dr. Varga Péter - GEK-es"", ""type"": ""normal"" },
  { ""date"": ""2026-04-17"", ""day"": ""P"", ""start"": ""12:30"", ""end"": ""15:50"", ""subject"": ""Villamos energetika és bizt. tech."", ""code"": ""GEVEE511-BL2"", ""room"": ""A1/320"", ""teacher"": ""Dr. Kozsely Gábor"", ""type"": ""potlo"" },
  { ""date"": ""2026-04-18"", ""day"": ""So"", ""start"": ""08:30"", ""end"": ""11:50"", ""subject"": ""Jelek és rendszerek elmélete"", ""code"": ""GEVAU220-ML"", ""room"": ""In/203"", ""teacher"": ""Móré Ádám"", ""type"": ""normal"" },

  { ""date"": ""2026-04-24"", ""day"": ""P"", ""start"": ""12:30"", ""end"": ""15:50"", ""subject"": ""Automaták és formális nyelvek"", ""code"": ""GEMAN385-ML"", ""room"": ""A1/14"", ""teacher"": ""Dr. Veres Laura"", ""type"": ""normal"" },
  { ""date"": ""2026-04-24"", ""day"": ""P"", ""start"": ""16:00"", ""end"": ""19:20"", ""subject"": ""Beágyazott rendszerek"", ""code"": ""GEVAU160-ML"", ""room"": ""In/200"", ""teacher"": ""Drótos Dániel"", ""type"": ""normal"" },
  { ""date"": ""2026-04-25"", ""day"": ""So"", ""start"": ""08:30"", ""end"": ""11:50"", ""subject"": ""Jelek és rendszerek elmélete"", ""code"": ""GEVAU220-ML"", ""room"": ""In/203"", ""teacher"": ""Aradi Attila"", ""type"": ""normal"" },

  { ""date"": ""2026-05-08"", ""day"": ""P"", ""start"": ""12:30"", ""end"": ""15:50"", ""subject"": ""Differenciálegyenletek"", ""code"": ""GEMAN500-ML"", ""room"": ""A1/320"", ""teacher"": ""Dr. Varga Péter - GEK-es"", ""type"": ""normal"" },
  { ""date"": ""2026-05-08"", ""day"": ""P"", ""start"": ""16:00"", ""end"": ""19:20"", ""subject"": ""Beágyazott rendszerek"", ""code"": ""GEVAU160-ML"", ""room"": ""In/200"", ""teacher"": ""Drótos Dániel"", ""type"": ""normal"" },
  { ""date"": ""2026-05-08"", ""day"": ""So"", ""start"": ""08:30"", ""end"": ""11:50"", ""subject"": ""Jelek és rendszerek elmélete"", ""code"": ""GEVAU220-ML"", ""room"": ""In/203"", ""teacher"": ""Aradi Attila"", ""type"": ""normal"" },
  { ""date"": ""2026-05-08"", ""day"": ""P"", ""start"": ""16:00"", ""end"": ""19:20"", ""subject"": ""Villamos energetika és bizt. tech."", ""code"": ""GEVEE511-BL2"", ""room"": ""A1/320"", ""teacher"": ""Dr. Kozsely Gábor"", ""type"": ""potlo"" },

  { ""date"": ""2026-05-15"", ""day"": ""P"", ""start"": ""12:30"", ""end"": ""15:50"", ""subject"": ""Automaták és formális nyelvek"", ""code"": ""GEMAN385-ML"", ""room"": ""X"", ""teacher"": ""Dr. Veres Laura"", ""type"": ""normal"" },
  { ""date"": ""2026-05-16"", ""day"": ""So"", ""start"": ""10:10"", ""end"": ""13:30"", ""subject"": ""Differenciálegyenletek"", ""code"": ""GEMAN500-ML"", ""room"": ""A1/320"", ""teacher"": ""Dr. Varga Péter - GEK-es"", ""type"": ""normal"" },
  { ""date"": ""2026-05-16"", ""day"": ""So"", ""start"": ""08:30"", ""end"": ""11:50"", ""subject"": ""Automatika"", ""code"": ""GEVAU502-BL2"", ""room"": ""XXXII"", ""teacher"": ""Móré Ádám"", ""type"": ""potlo"" }
]";

            var items = JsonSerializer.Deserialize<List<RawScheduleItem>>(json);

            foreach (var item in items)
            {
                // 1. Megnézzük, létezik-e már a tantárgy a DB-ben
                var subject = context.Subjects.FirstOrDefault(s => s.Code == item.code);

                if (subject == null)
                {
                    subject = new Subject
                    {
                        Name = item.subject,
                        Code = item.code,
                        Teacher = item.teacher
                    };
                    context.Subjects.Add(subject);
                    context.SaveChanges(); // Kell a mentés, hogy legyen ID-ja
                }

                // 2. Létrehozzuk az órát és hozzákötjük a tárgyhoz
                var session = new ClassSession
                {
                    SubjectId = subject.Id,
                    Room = item.room,
                    Type = item.type,
                    // Dátum konverzió (String -> DateTime)
                    StartTime = DateTime.Parse($"{item.date} {item.start}"),
                    EndTime = DateTime.Parse($"{item.date} {item.end}")
                };
                context.Sessions.Add(session);
            }
            context.SaveChanges();
        }
    }
}
