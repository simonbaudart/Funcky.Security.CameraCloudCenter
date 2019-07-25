import React from "react";
import {FootageDetail} from "./FootageDetail";
import {Footage} from "../../Models";

interface FootageListRecordingProps
{
    footage: Footage;
    cameraName: string;
}

export const FootageListRecording = (props: FootageListRecordingProps) =>
{
    return <>
        <div>
            <h3>
                {props.footage.title}
            </h3>
            {
                props.footage.sequences.map((footage) =>
                {
                    return (<div key={footage.id} className="">
                        <FootageDetail footage={footage} cameraName={props.cameraName}/>
                    </div>);
                })
            }
        </div>
    </>;
};