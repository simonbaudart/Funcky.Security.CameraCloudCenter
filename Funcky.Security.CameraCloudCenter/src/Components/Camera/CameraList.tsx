import React from "react";
import { Camera } from "../../Models";
import { CameraSummary } from "../";

interface CameraListProps {
    cameras: Camera[];
}

export const CameraList = (props: CameraListProps) => {
    return <div className="row pb-3">
        {
            props.cameras.map((camera) => {
                return (<div key={camera.key} className="col-12 col-md-6 col-xl-3">
                    <CameraSummary camera={camera} />
                </div>);
            })
        }
    </div>;
};