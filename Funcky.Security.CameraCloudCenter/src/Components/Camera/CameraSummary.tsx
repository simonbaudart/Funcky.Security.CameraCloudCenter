import React from "react";
import {Camera} from "../../Models";
import {ContextAwareProps, withContext} from "../../Hoc";

interface CameraSummaryProps extends ContextAwareProps
{
    camera: Camera;
    selectCamera: (camera: Camera) => void;
}

const CameraSummaryComponent = (props: CameraSummaryProps) =>
{

    return <div className="card mb-3">
        <div className="card-body w-100">
            <img className="card-img-top" src={props.camera.lastFootageImageUrl}
                 alt={"Last footage " + props.camera.name}/>
            <h5 className="card-title text-center">{props.camera.name}</h5>
            <p className="card-text text-center">
                <b>Last footage date</b><br/>
                {Intl.DateTimeFormat("fr-BE",
                    {
                        year: '2-digit',
                        month: '2-digit',
                        day: '2-digit',
                        hour: '2-digit',
                        minute: '2-digit',
                        second: '2-digit'
                    }).format(Date.parse(props.camera.lastFootageDate))}
            </p>
            <a href="#" className="btn btn-primary w-100" onClick={(e) =>
            {
                e.preventDefault();
                props.selectCamera(props.camera);
            }}>Show details</a>
        </div>
    </div>;
};

export const CameraSummary = withContext<CameraSummaryProps>(CameraSummaryComponent);