import { useEffect, useState } from 'react'
import '../App.css'
import FullCalendar from '@fullcalendar/react';
import { Box, CircularProgress, Typography, Paper, Button, Dialog, DialogActions, DialogContent, DialogTitle, Divider, List, ListItem, ListItemText, TextField } from '@mui/material';
import dayGridPlugin from '@fullcalendar/daygrid' // Havi nézet
import timeGridPlugin from '@fullcalendar/timegrid' // Heti/Napi nézet
import interactionPlugin from '@fullcalendar/interaction' // Kattintás kezelés
import type { Attachment } from '../models/Attachment';
import type { Note } from '../models/Note';
import type { SessionEvent } from '../models/SessionEvent';

function Calendar() {
    const [events, setEvents] = useState<SessionEvent[]>([]);
    const [loading, setLoading] = useState(true);

    const [open, setOpen] = useState(false);
    const [selectedEvent, setSelectedEvent] = useState<any>(null); // A kiválasztott óra adatai
    const [notes, setNotes] = useState<Note[]>([]); // A betöltött jegyzetek
    const [newNote, setNewNote] = useState(""); // Az éppen írt szöveg

    const [attachments, setAttachments] = useState<Attachment[]>([]);
    const [uploading, setUploading] = useState(false);

    // Adatok betöltése az API-ból
    useEffect(() => {
        fetch('/api/schedule')
            .then(res => res.json())
            .then(data => {
                setEvents(data);
                setLoading(false);
                console.log("Betöltött órák:", data);
            })
            .catch(err => {
                console.error("Hiba a betöltéskor:", err);
                setLoading(false);
            });
            console.log("useEffect lefutott, események:", events);
    }, []);

    // 2. Ha rákattintunk egy órára -> Modal megnyitása
    const handleEventClick = (info: any) => {
        const eventData = info.event;
        setSelectedEvent({
            id: eventData.id,
            title: eventData.title,
            start: eventData.start,
            room: eventData.extendedProps.room,
            teacher: eventData.extendedProps.teacher,
            subjectId: eventData.extendedProps.subjectId
        });

        // Meglévő jegyzetek betöltése
        fetch(`/api/notes/session/${eventData.id}`)
            .then(res => res.json())
            .then(data => setNotes(data));

        fetch(`/api/attachments/session/${eventData.id}`)
            .then(res => res.json())
            .then(data => setAttachments(data));

        setOpen(true);
    };

    // 3. Új jegyzet mentése
    const handleSaveNote = () => {
        if (!newNote.trim()) return;

        const notePayload = {
            content: newNote,
            classSessionId: parseInt(selectedEvent.id),
            // subjectId: selectedEvent.subjectId // Opcionális: ha tárgyhoz is akarod kötni
        };

        fetch('/api/notes', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(notePayload)
        })
            .then(res => res.json())
            .then(savedNote => {
                setNotes([savedNote, ...notes]); // Hozzáadjuk a listához azonnal
                setNewNote(""); // Töröljük a mezőt
            });
    };

    const handleDeleteNote = (noteId: number) => {
        fetch(`/api/notes/${noteId}`, { method: 'DELETE' })
            .then(() => {
                setNotes(notes.filter(note => note.id !== noteId)); // Frissítjük a listát
            });
    };

    const handleClose = () => {
        setOpen(false);
        setNewNote("");
        setNotes([]);
    };

    const handleFileUpload = (event: React.ChangeEvent<HTMLInputElement>) => {
        if (!event.target.files || event.target.files.length === 0) return;

        const file = event.target.files[0];
        const formData = new FormData();
        formData.append("file", file);
        formData.append("classSessionId", selectedEvent.id);

        setUploading(true);

        fetch('/api/attachments', {
            method: 'POST',
            body: formData // Itt nem kell Content-Type fejléc, a böngésző intézi!
        })
            .then(res => {
                if (!res.ok) throw new Error("Feltöltési hiba");
                return res.json();
            })
            .then(newAttachment => {
                setAttachments([...attachments, newAttachment]);
                setUploading(false);
            })
            .catch(err => {
                console.error(err);
                setUploading(false);
                alert("Sikertelen feltöltés!");
            });
    };

    if (loading) return <Box display="flex" justifyContent="center" mt={5}><CircularProgress /></Box>;

    return (
        <Box sx={{ p: 2, height: '100vh', display: 'flex', flexDirection: 'column' }}>
            <Typography variant="h4" gutterBottom component="div" sx={{ fontWeight: 'bold', color: '#1976d2' }}>
                UniBrain 🧠
            </Typography>

            <Paper elevation={3} sx={{ flex: 1, p: 2 }}>
                <FullCalendar
                    plugins={[timeGridPlugin, dayGridPlugin, interactionPlugin]}
                    initialView="timeGridWeek"

                    // Hétvégi levelezős beállítások:
                    weekends={true}
                    hiddenDays={[1, 2, 3, 4]} // H-K-Sz-Cs elrejtése (opcionális, ha csak hétvégén van órád)
                    slotMinTime="08:00:00"    // Reggel 8-tól
                    slotMaxTime="20:00:00"    // Este 8-ig

                    locale="hu"
                    headerToolbar={{
                        left: 'prev,next today',
                        center: 'title',
                        right: 'dayGridMonth,timeGridWeek'
                    }}

                    events={events}
                    eventClick={handleEventClick}
                    height="100%"
                />
            </Paper>

            <Dialog open={open} onClose={handleClose} fullWidth maxWidth="sm">
                {selectedEvent && (
                    <>
                        <DialogTitle sx={{ bgcolor: '#1976d2', color: 'white' }}>
                            {selectedEvent.title}
                        </DialogTitle>
                        <DialogContent sx={{ mt: 2 }}>
                            <Box sx={{ mb: 3, mt: 1 }}>
                                <Typography variant="body1"><strong>👨‍🏫 Tanár:</strong> {selectedEvent.teacher}</Typography>
                                <Typography variant="body1"><strong>🏫 Terem:</strong> {selectedEvent.room}</Typography>
                                <Typography variant="body2" color="textSecondary">
                                    ⏰ Kezdés: {new Date(selectedEvent.start).toLocaleString('hu-HU')}
                                </Typography>
                            </Box>

                            <Divider sx={{ mb: 2 }} />

                            <Typography variant="h6" gutterBottom sx={{ mt: 2 }}>📎 Csatolmányok</Typography>

                            {/* Fájl feltöltés gomb */}
                            <Button
                                variant="outlined"
                                component="label"
                                disabled={uploading}
                                sx={{ mb: 2 }}
                            >
                                {uploading ? "Feltöltés..." : "+ Fájl csatolása"}
                                <input
                                    type="file"
                                    hidden
                                    onChange={handleFileUpload}
                                />
                            </Button>

                            {/* Fájlok listája */}
                            <List dense>
                                {attachments.map((att) => (
                                    <ListItem key={att.id}>
                                        <ListItemText
                                            primary={
                                                <a
                                                    href={`/uploads/${att.storedFileName}`}
                                                    target="_blank"
                                                    rel="noopener noreferrer"
                                                    style={{ textDecoration: 'none', color: '#1976d2', fontWeight: 'bold' }}
                                                >
                                                    📄 {att.fileName}
                                                </a>
                                            }
                                        />
                                    </ListItem>
                                ))}
                            </List>

                            <Divider sx={{ mb: 2 }} />

                            <Typography variant="h6" gutterBottom>📝 Jegyzetek</Typography>

                            {/* Jegyzet írása */}
                            <TextField
                                autoFocus
                                margin="dense"
                                label="Írj egy gyors jegyzetet..."
                                fullWidth
                                multiline
                                rows={3}
                                variant="outlined"
                                value={newNote}
                                onChange={(e) => setNewNote(e.target.value)}
                            />
                            <Button onClick={handleSaveNote} variant="contained" sx={{ mt: 1, mb: 2 }}>
                                Mentés
                            </Button>

                            {/* Korábbi jegyzetek listája */}
                            <List sx={{ bgcolor: '#f5f5f5', borderRadius: 1 }}>
                                {notes.length === 0 && <Typography variant="body2" sx={{ p: 2, fontStyle: 'italic' }}>Nincs még jegyzet.</Typography>}

                                {notes.map((note) => (
                                    <ListItem key={note.id} alignItems="flex-start">
                                        <ListItemText
                                            primary={note.content}
                                            secondary={new Date(note.createdAt).toLocaleString('hu-HU')}
                                        />
                                        <Button onClick={() => handleDeleteNote(note.id)}>
                                            <Typography variant="button">Törlés</Typography>
                                        </Button>
                                    </ListItem>
                                ))}
                            </List>

                        </DialogContent>
                        <DialogActions>
                            <Button onClick={handleClose}>Bezárás</Button>
                        </DialogActions>
                    </>
                )}
            </Dialog>
        </Box>
    )
}

export default Calendar;
