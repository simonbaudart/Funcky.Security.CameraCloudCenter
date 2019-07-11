import React from "react";
import { Camera } from "../../Models/Camera";
import { CameraSummary } from "../Camera/CameraSummary";

interface SideBarProps {
    cameras: Camera[];
}

export const SideBar = (props: SideBarProps) => {
    return <div>
        {
            props.cameras.map((camera, i) => {
                return (<CameraSummary key={i} camera={camera} />);
            })
        }
    </div>;
};