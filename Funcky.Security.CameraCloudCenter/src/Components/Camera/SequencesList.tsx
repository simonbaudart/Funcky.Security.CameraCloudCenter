import React from "react";
import { Footage, FootageUrl } from "../../Models";

import Events from "../../Stores/Events";
import CameraStore from "../../Stores/CameraStore";
import CameraActions from "../../Stores/CameraActions";

interface SequencesListProps
{
}

interface SequencesListState
{
    currentFootage: Footage | null;

    currentSequence: Footage | null;
    currentSequenceIndex: number;
    currentSequenceUrl: FootageUrl | null;
}

export class SequencesList extends React.Component<SequencesListProps, SequencesListState>
{
    constructor(props: SequencesListProps)
    {
        super(props);

        const content = CameraStore.getContent();

        this.state = {
            currentFootage: content.currentFootage,
            currentSequence: content.currentSequence,
            currentSequenceIndex: content.currentSequenceIndex,
            currentSequenceUrl: content.currentSequenceUrl
        };
    }

    componentDidMount()
    {
        CameraStore.addChangeListener(Events.CameraListLoaded, () => this.getStateFromStore());
        CameraStore.addChangeListener(Events.CameraFootagesLoaded, () => this.getStateFromStore());
        CameraStore.addChangeListener(Events.CameraSequencesLoaded, () => this.getStateFromStore());
    }

    private getStateFromStore()
    {
        var content = CameraStore.getContent();

        this.setState({
            currentFootage: content.currentFootage,
            currentSequence: content.currentSequence,
            currentSequenceIndex: content.currentSequenceIndex,
            currentSequenceUrl: content.currentSequenceUrl
        });
    }

    render()
    {
        if (!this.state.currentSequence || !this.state.currentSequenceUrl || !this.state.currentFootage)
        {
            return <></>;
        }

        let footageDetail = <></>;

        switch (this.state.currentSequenceUrl.type)
        {
            case "snap":
                footageDetail = <img className="w-100 card-img-top" src={this.state.currentSequenceUrl.url}
                    alt={this.state.currentSequence.title} />;
                break;
            case "recording":
                footageDetail = <div>
                    <video className="w-100" src={this.state.currentSequenceUrl.url} autoPlay controls
                        onError={(error: any) => console.log(error.target.error)}>
                    </video>
                    <a href={this.state.currentSequenceUrl.url}>{this.state.currentSequence.title}</a>
                </div>;
                break;
            default:
                footageDetail = <div>{this.state.currentSequenceUrl.url}</div>;
                break;
        }

        let footageNavigation = <div className="row">
            <div className="col-6">
                <button type="button" className="btn btn-secondary w-100 p-2"
                    onClick={(e) =>
                    {
                        e.preventDefault();
                        CameraActions.previousSequence();
                    }}>Previous</button>
            </div>
            <div className="col-6">
                <button type="button" className="btn btn-primary w-100 p-2"
                    onClick={(e) =>
                    {
                        e.preventDefault();
                        CameraActions.nextSequence();
                    }}>Next</button>
            </div>
        </div>;

        let details = <div className="card">

            {footageDetail}

            <div className="card-body">
                <div>
                    <span className="badge badge-primary">
                        {this.state.currentSequenceIndex + 1} / {this.state.currentFootage.sequences.length}
                    </span>
                </div>
                <div>
                    <b>
                        {this.state.currentFootage.title}
                    </b>
                </div>

                {footageNavigation}
            </div>
        </div>;

        return <>
            <div>
                <h3>
                    {this.state.currentFootage.title}
                </h3>

                {details}

            </div>
        </>
    }
};