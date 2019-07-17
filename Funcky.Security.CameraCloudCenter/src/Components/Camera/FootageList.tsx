import React from "react";
import { Footage } from "../../Models";

import { FootageDetail } from "../";

export interface FootageListProps
{
    footage: Footage | undefined;
    cameraName: string;
}

export const FootageList = (props: FootageListProps) =>
{
    let content = <div></div>;

    if (props.footage)
    {
        content = <div>
            <h3>
                {props.footage.title}
            </h3>
            {
                props.footage.sequences.map((footage) =>
                {
                    return (<div key={footage.id} className="">
                        <FootageDetail footage={footage} cameraName={props.cameraName} />
                    </div>);
                })
            }
        </div>;
    }

    return content;
}