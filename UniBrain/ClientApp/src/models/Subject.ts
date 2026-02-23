import type { Note } from "./Note";
import type { SessionEvent } from "./SessionEvent";

export interface Subject {
    id: number;
    name: string;
    code: string;
    teacher?: string;

    sessions: SessionEvent[];
    notes: Note[];
}        