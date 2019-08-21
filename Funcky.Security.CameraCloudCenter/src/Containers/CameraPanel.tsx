import React from "react";

import { CameraDetail, CameraList } from "../Components";
import CameraActions from "../Stores/CameraActions";

interface CameraPanelProps
{
}

interface CameraPanelState
{
}

export class CameraPanel extends React.Component<CameraPanelProps, CameraPanelState>
{
    constructor(props: CameraPanelProps)
    {
        super(props);

        document.addEventListener('keydown', this.handleKeyDown.bind(this));
    }

    private handleKeyDown(e)
    {
        switch (e.keyCode)
        {
            case 37: //left
                e.preventDefault();
                CameraActions.previousSequence();
                break;
            case 38: //up
                CameraActions.previousFootage();
                e.preventDefault();
                break;
            case 39: //right
                e.preventDefault();
                CameraActions.nextSequence();
                break;
            case 40: //down
                CameraActions.nextFootage();
                e.preventDefault();
                break;
        }
    }

    render()
    {
        return <>
            <div className="row pb-3">
                <div className="col-12 col-lg-3">
                    <CameraList />
                </div>
                <div className="col-12 col-lg-9">
                    <CameraDetail />
                </div>
            </div>
        </>;
    }
}