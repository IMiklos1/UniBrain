export interface SessionEvent {
    id: string;
    title: string;
    start: string;
    end: string;
    backgroundColor?: string;
    extendedProps: {
        room: string;
        teacher: string;
        subjectId: number;
    }
}