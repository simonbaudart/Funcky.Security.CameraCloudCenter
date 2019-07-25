import React from "react";
import {Camera} from "../../Models";
import {CameraSummary} from "../";

interface CameraListProps
{
    cameras: Camera[];
}

export const CameraList = (props: CameraListProps) =>
{
    return <>
        {
            props.cameras.map((camera) =>
            {
                return (
                    <div key={camera.key} className="row pb-3">
                        <div className="col-12">
                            <CameraSummary camera={camera}/>
                        </div>
                    </div>);
            })
        }
    </>;
};