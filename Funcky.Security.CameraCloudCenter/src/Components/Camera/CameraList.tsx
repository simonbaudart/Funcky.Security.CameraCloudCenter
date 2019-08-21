import React from "react";
import { Camera } from "../../Models";
import { CameraSummary } from "../";

import Events from "../../Stores/Events";
import CameraStore from "../../Stores/CameraStore";

interface CameraListProps
{
}

interface CameraListState
{
    cameras: Camera[];
}

export class CameraList extends React.Component<CameraListProps, CameraListState>
{
    constructor(props: CameraListProps)
    {
        super(props);

        var content = CameraStore.getContent();

        this.state = {
            cameras: content.cameras
        };
    }

    componentDidMount()
    {
        CameraStore.addChangeListener(Events.CameraListLoaded, () => this.setState({ cameras: CameraStore.getContent().cameras }));
    }

    render()
    {
        return <>
            {
                this.state.cameras.map((camera) =>
                {
                    return (
                        <div key={camera.key} className="row pb-3">
                            <div className="col-12">
                                <CameraSummary camera={camera} />
                            </div>
                        </div>);
                })
            }
        </>;
    }
}
/*

export const CameraList = (props: CameraListProps) =>
{
    return <>
        {
            props.cameras.map((camera) =>
            {
                return (
                    <div key={camera.key} className="row pb-3">
                        <div className="col-12">
                            <CameraSummary camera={camera} selectCamera={props.selectCamera}/>
                        </div>
                    </div>);
            })
        }
    </>;
};

*/