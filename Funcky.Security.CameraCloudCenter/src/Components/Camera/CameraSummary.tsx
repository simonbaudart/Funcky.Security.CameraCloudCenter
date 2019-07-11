import React from "react";
import { Camera } from "../../Models/Camera";

interface CameraSummaryProps {
    camera: Camera;
}
export const CameraSummary = (props: CameraSummaryProps) => {
    return <div className="card mb-3">
        <div className="card-body">
            <h5 className="card-title">{props.camera.name}</h5>
            <p className="card-text">
                <b>Last footage date : {Intl.DateTimeFormat("fr-BE",
                    {
                        year: 'numeric',
                        month: 'numeric',
                        day: 'numeric'
                    }).format(Date.parse(props.camera.lastFootageDate))} </b>
            </p>
            <a href="#" className="btn btn-primary">Go somewhere</a>
        </div>
    </div>;
}