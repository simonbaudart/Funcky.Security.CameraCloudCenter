import React from "react";
// ReSharper disable once UnusedLocalImport
import format from 'date-fns/format';

import {Camera, Footage, FootageUrl} from "../../Models";

import {FootageList} from "../";

interface CameraDetailProps
{
    camera: Camera;
    displayedDate: Date;

    footages: Footage[];
    selectedFootage? : Footage;

    currentFootage: Footage | undefined;
    currentFootageUrl: FootageUrl | undefined;
    currentFootageIndex: number | undefined;

    addDays: (jump: number) => void;
    selectFootage: (footage: Footage) => void;
    moveFootage: (jump: number) => void;
    previousFootage: () => void;
    nextFootage: () => void;
}

export const CameraDetail = (props: CameraDetailProps) => {

    const displayFootageList = () => {
        let footageList = <></>;

        if (props.selectedFootage)
        {
            footageList = <FootageList selectedFootage={props.selectedFootage} 
                                       cameraName={props.camera.key}
                                       previousFootage={props.previousFootage}
                                       nextFootage={props.nextFootage}
                                       currentFootage={props.currentFootage}
                                       currentFootageIndex={props.currentFootageIndex}
                                       currentFootageUrl={props.currentFootageUrl}
                                       moveFootage={props.moveFootage}
            />;
        }
        
        return footageList;
    };
    
    return <div>
        <div className="row">
            <div className="col">
                <h2>{props.camera.name}</h2>
            </div>
        </div>

        <div className="row">
            <div className="col-12">
                <h3>
                    {format(props.displayedDate, 'DD/MM/YYYY')}
                    <button type="button" className="btn btn-secondary ml-2"
                            onClick={(e) =>{e.preventDefault(); props.addDays(-1);}}>&lt;</button>
                    <button type="button" className="btn btn-primary"
                            onClick={(e) => {e.preventDefault(); props.addDays(1);}}>&gt;</button>
                </h3>

            </div>
        </div>

        <div className="row">
            <div className="col-12 col-lg-6 m-auto">
                {displayFootageList()}
            </div>
        </div>

        <div className="row pb-3">
            <div className="list-group">
                {
                    props.footages.map((footage) =>
                    {
                        return <button key={footage.id} type="button" className={
                            "list-group-item list-group-item-action " +
                            (props.selectedFootage && props.selectedFootage === footage ? "active" : "")
                        } onClick={(e) => {e.preventDefault(); props.selectFootage(footage)}}>
                            {footage.title}
                        </button>
                    })
                }
            </div>
        </div>
    </div>;
};