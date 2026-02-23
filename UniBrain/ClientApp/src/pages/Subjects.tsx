import { useEffect, useState } from "react";
import SubjectCard from "../components/SubjectCard";

function Subjects() {
    const [subjects, setSubjects] = useState([]);

    useEffect(() => {
        document.title = "UniBrain - Subjects";

        fetch('/api/subjects')
            .then(res => res.json())
            .then(data => {
                setSubjects(data);
                console.log("Betöltött tantárgyak:", data);
            })
            .catch(err => {
                console.error("Hiba a tantárgyak betöltésekor:", err);
            });
    }, []);

    return (
        <div>
            <h1>Subjects</h1>
            <ul>
                {subjects.map((subject: any) => (
                    <SubjectCard subject={subject}/>
                ))}
            </ul>
        </div>
    );
}

export default Subjects;