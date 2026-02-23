import type { Subject } from "../models/Subject";

function SubjectCard({ subject }: { subject: Subject }) {
    return (
        <div className="subject-card">
            <h2>{subject.name}</h2>
            <dl>
                {Object.entries(subject).map(([key, value]) => (
                    <div key={key}>
                        <dt>{key}</dt>
                        <dd>{String(value)}</dd>
                    </div>
                ))}
            </dl>
            <p>{subject.code}</p>
        </div>
    );
}

export default SubjectCard;