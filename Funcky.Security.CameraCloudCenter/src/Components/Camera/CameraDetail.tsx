import React from "react";
// ReSharper disable once UnusedLocalImport
import format from 'date-fns/format';

import { Camera, Footage } from "../../Models";

import Events from "../../Stores/Events";
import CameraStore from "../../Stores/CameraStore";

import { SequencesList } from "./SequencesList";

interface CameraDetailProps
{
}

interface CameraDetailState
{
    displayedDate: Date;
    currentCamera: Camera | null;

    footages: Footage[] | null;
    currentFootage: Footage | null;
}

export class CameraDetail extends React.Component<CameraDetailProps, CameraDetailState>
{
    constructor(props: CameraDetailProps)
    {
        super(props);

        const content = CameraStore.getContent();

        this.state = {
            displayedDate: content.displayedDate,
            currentCamera: content.currentCamera,
            footages: content.footages,
            currentFootage: content.currentFootage
        };
    }

    componentDidMount()
    {
        CameraStore.addChangeListener(Events.CameraListLoaded, () => this.getStateFromStore());
        CameraStore.addChangeListener(Events.CameraFootagesLoaded, () => this.getStateFromStore());
    }

    private getStateFromStore()
    {
        var content = CameraStore.getContent();

        this.setState({
            displayedDate: content.displayedDate,
            currentCamera: content.currentCamera,
            footages: content.footages,
            currentFootage: content.currentFootage
        });
    }

    render()
    {
        if (!this.state.currentCamera || !this.state.footages)
        {
            return <></>;
        }

        return <div>
            <div className="row">
                <div className="col">
                    <h2>{this.state.currentCamera.name}</h2>
                </div>
            </div>

            <div className="row">
                <div className="col-12">
                    <h3>
                        {format(this.state.displayedDate, 'DD/MM/YYYY')}
                        <button type="button" className="btn btn-secondary ml-2"
                            onClick={(e) =>
                            {
                                e.preventDefault();
                                // TODO : props.jumpDays(-1);
                            }}>&lt;</button>
                        <button type="button" className="btn btn-primary"
                            onClick={(e) =>
                            {
                                e.preventDefault();
                                // TODO : props.jumpDays(1);
                            }}>&gt;</button>
                    </h3>

                </div>
            </div>

            <div className="row">
                <div className="col-12 col-lg-6 m-auto">
                    <SequencesList />
                </div>
            </div>

            <div className="row pb-3">
                <div className="list-group">
                    {
                        this.state.footages.map((footage) =>
                        {
                            return <button key={footage.id} type="button" className={
                                "list-group-item list-group-item-action " +
                                (this.state.currentFootage && this.state.currentFootage === footage ? "active" : "")
                            } onClick={(e) =>
                            {
                                e.preventDefault();
                                // TODO : props.selectFootage(footage, 0);
                            }}>
                                {footage.title}
                            </button>;
                        })
                    }
                </div>
            </div>
        </div>;
    }
};