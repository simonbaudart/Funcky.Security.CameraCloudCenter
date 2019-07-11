import React from "react";
import { Camera } from "../../Models/Camera";
import { CameraSummary } from "../Camera/CameraSummary";

interface SideBarProps {
    cameras: Camera[];
}

export const SideBar = (props: SideBarProps) => {
    return <>
        {
            props.cameras.map((camera, i) => {
                return (<div className="col-12 col-md-6 col-xl-3"><CameraSummary key={i} camera={camera} /></div>);
            })
        }
        </>;
};