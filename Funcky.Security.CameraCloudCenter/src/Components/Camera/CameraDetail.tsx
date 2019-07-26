import React from "react";
// ReSharper disable once UnusedLocalImport
import format from 'date-fns/format';

import {Camera, Footage, FootageUrl} from "../../Models";

import {SequencesList} from "../";

interface CameraDetailProps
{
    currentCamera: Camera;
    displayedDate: Date;

    footages: Footage[];
    currentFootage?: Footage;

    currentSequence: Footage | undefined;
    currentSequenceUrl: FootageUrl | undefined;
    currentSequenceIndex: number | undefined;

    jumpDays: (jump: number) => void;
    selectFootage: (footage: Footage, sequenceIndex: 0) => void;

    jumpSequence: (jump: number) => void;
}

export const CameraDetail = (props: CameraDetailProps) =>
{

    const displaySequencesList = () =>
    {
        let footageList = <></>;

        if (props.currentFootage && props.currentSequence && props.currentSequenceUrl && (props.currentSequenceIndex || props.currentSequenceIndex === 0))
        {
            footageList = <SequencesList currentFootage={props.currentFootage}
                                         currentSequence={props.currentSequence}
                                         currentSequenceUrl={props.currentSequenceUrl}
                                         currentSequenceIndex={props.currentSequenceIndex}
                                         jumpSequence={props.jumpSequence}
            />;
        }

        return footageList;
    };

    return <div>
        <div className="row">
            <div className="col">
                <h2>{props.currentCamera.name}</h2>
            </div>
        </div>

        <div className="row">
            <div className="col-12">
                <h3>
                    {format(props.displayedDate, 'DD/MM/YYYY')}
                    <button type="button" className="btn btn-secondary ml-2"
                            onClick={(e) =>
                            {
                                e.preventDefault();
                                props.jumpDays(-1);
                            }}>&lt;</button>
                    <button type="button" className="btn btn-primary"
                            onClick={(e) =>
                            {
                                e.preventDefault();
                                props.jumpDays(1);
                            }}>&gt;</button>
                </h3>

            </div>
        </div>

        <div className="row">
            <div className="col-12 col-lg-6 m-auto">
                {displaySequencesList()}
            </div>
        </div>

        <div className="row pb-3">
            <div className="list-group">
                {
                    props.footages.map((footage) =>
                    {
                        return <button key={footage.id} type="button" className={
                            "list-group-item list-group-item-action " +
                            (props.currentFootage && props.currentFootage === footage ? "active" : "")
                        } onClick={(e) =>
                        {
                            e.preventDefault();
                            props.selectFootage(footage, 0)
                        }}>
                            {footage.title}
                        </button>
                    })
                }
            </div>
        </div>
    </div>;
};