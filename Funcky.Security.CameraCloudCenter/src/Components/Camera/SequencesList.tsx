import React from "react";
import {Footage, FootageUrl} from "../../Models";

interface SequencesListProps
{
    currentFootage: Footage;

    currentSequence: Footage;
    currentSequenceUrl: FootageUrl;
    currentSequenceIndex: number;

    jumpSequence: (jump: number) => void;
}

export const SequencesList = (props: SequencesListProps) =>
{

    const displayFootageDetails = () =>
    {
        let content = <></>;

        switch (props.currentSequenceUrl.type)
        {
            case "snap":
                content = <img className="w-100 card-img-top" src={props.currentSequenceUrl.url}
                               alt={props.currentSequence.title}/>;
                break;
            case "recording":
                content = <div>
                    <video className="w-100" src={props.currentSequenceUrl.url} autoPlay controls
                           onError={(error: any) => console.log(error.target.error)}>
                    </video>
                    <a href={props.currentSequenceUrl.url}>{props.currentSequence.title}</a>
                </div>;
                break;
            default:
                content = <div>{props.currentSequenceUrl.url}</div>;
                break;
        }

        return content;
    };

    const displayDetails = () =>
    {
        let footageNavigation = <div className="row">
            <div className="col-6">
                <button type="button" className="btn btn-secondary w-100 p-2"
                        onClick={(e) =>
                        {
                            e.preventDefault();
                            props.jumpSequence(-1);
                        }}>Previous
                </button>
            </div>
            <div className="col-6">
                <button type="button" className="btn btn-primary w-100 p-2"
                        onClick={(e) =>
                        {
                            e.preventDefault();
                            props.jumpSequence(1);
                        }}>Next
                </button>
            </div>
        </div>;

        let details = <div className="card">
            <img className="card-img-top" src="/img/1280x720.gif" alt="No footage selected"/>

            <div className="card-body">
                <div>
                        <span className="badge badge-primary">
                            {props.currentSequenceIndex + 1} / {props.currentFootage.sequences.length}
                        </span>
                </div>
                <div>
                    <b>
                        {props.currentFootage.title}
                    </b>
                </div>

                {footageNavigation}
            </div>
        </div>;

        details = <div className="card">

            {displayFootageDetails()}

            <div className="card-body">
                <div>
                        <span className="badge badge-primary">
                            {props.currentSequenceIndex + 1} / {props.currentFootage.sequences.length}
                        </span>
                </div>
                <div>
                    <b>
                        {props.currentFootage.title}
                    </b>
                </div>

                {footageNavigation}
            </div>
        </div>;

        return details;
    };

    return <>
        <div>
            <h3>
                {props.currentFootage.title}
            </h3>

            {displayDetails()}

        </div>
    </>
};