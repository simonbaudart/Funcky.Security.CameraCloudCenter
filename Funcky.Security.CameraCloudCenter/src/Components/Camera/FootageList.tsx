import React from "react";
import { Footage } from "../../Models";

interface FootageListProps
{
    footage: Footage;
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
        </div>;
    }

    return content;
}