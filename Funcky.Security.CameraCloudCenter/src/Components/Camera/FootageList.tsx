import React from "react";
import {Footage, FootageTypes} from "../../Models";

import {FootageListRecording, FootageListSnap} from "../";

export interface FootageListProps
{
    footage: Footage | undefined;
    cameraName: string;
}

export const FootageList = (props: FootageListProps) =>
{
    let content = <></>;

    if (props.footage && props.footage.type === FootageTypes.video)
    {
        content = <FootageListRecording footage={props.footage} cameraName={props.cameraName} />;
    }
    else if (props.footage && props.footage.type === FootageTypes.picture)
    {
        content = <FootageListSnap footage={props.footage} cameraName={props.cameraName} />;
    }

    return content;
};