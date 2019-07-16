import React from "react";
import { Camera } from "../../Models";

interface CameraDetailProps {
    camera: Camera;
}
export const CameraDetail = (props: CameraDetailProps) => {

    return <div className="row pb-3">
        <div className="col">
            <h2>{props.camera.name}</h2>
        </div>
    </div>;
}
