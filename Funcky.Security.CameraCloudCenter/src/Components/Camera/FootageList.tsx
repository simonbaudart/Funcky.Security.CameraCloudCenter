import React from "react";
import {Footage, FootageUrl} from "../../Models";

interface FootageListProps
{
    selectedFootage: Footage;

    currentFootage: Footage | undefined;
    currentFootageUrl: FootageUrl | undefined;
    currentFootageIndex: number | undefined;

    cameraName: string;
    moveFootage: (jump: number) => void;
    nextFootage: () => void;
    previousFootage: () => void;
}

export const FootageList = (props: FootageListProps) =>
{

    const displayFootageDetails = () =>
    {
        let content = <></>;

        if (!props.currentFootageUrl || !props.currentFootageIndex || !props.currentFootage)
        {
            return content;
        }

        switch (props.currentFootageUrl.type)
        {
            case "snap":
                content = <img className="w-100 card-img-top" src={props.currentFootageUrl.url}
                               alt={props.selectedFootage.title}/>;
                break;
            case "recording":
                content = <div>
                    <video className="w-100" src={props.currentFootageUrl.url} autoPlay controls
                           onError={(error: any) => console.log(error.target.error)}>
                    </video>
                    <a href={props.currentFootageUrl.url}>{props.selectedFootage.title}</a>
                </div>;
                break;
            default:
                content = <div>{props.currentFootageUrl.url}</div>;
                break;
        }

        return content;
    };

    const displayDetails = () =>
    {
        let footageNavigation = <></>;

        if (!props.currentFootageUrl || !props.currentFootageIndex || !props.currentFootage)
        {
            return footageNavigation;
        }
        
        footageNavigation = <div className="row">
            <div className="col-6">
                <button type="button" className="btn btn-secondary w-100 p-2"
                        onClick={(e) =>
                        {
                            e.preventDefault();
                            props.moveFootage(-1);
                        }}>Previous
                </button>
            </div>
            <div className="col-6">
                <button type="button" className="btn btn-primary w-100 p-2"
                        onClick={(e) =>
                        {
                            e.preventDefault();
                            props.moveFootage(1);
                        }}>Next
                </button>
            </div>
        </div>;
        
        let details = <div className="card">
            <img className="card-img-top" src="/img/1280x720.gif" alt="No footage selected"/>

            <div className="card-body">
                <div>
                        <span className="badge badge-primary">
                            {props.currentFootageIndex + 1} / {props.selectedFootage.sequences.length}
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

        if (props.currentFootageUrl)
        {
            details = <div className="card">

                {displayFootageDetails}

                <div className="card-body">
                    <div>
                        <span className="badge badge-primary">
                            {props.currentFootageIndex + 1} / {props.selectedFootage.sequences.length}
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
        }

        return details;
    };

    return <>
        <div>
            <h3>
                {props.selectedFootage.title}
            </h3>

            {displayDetails}

        </div>
    </>
};