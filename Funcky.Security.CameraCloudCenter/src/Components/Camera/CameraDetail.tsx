import React from "react";
import moment from "moment";

import FullCalendar from '@fullcalendar/react';
import listPlugin from '@fullcalendar/list';

import { Camera, Footage } from "../../Models";
import { AjaxService } from "../../Services";

import { FootageList } from "../";

interface CameraDetailProps
{
    camera: Camera;
}

interface CameraDetailState
{
    selectedFootage: Footage | undefined;
}

export class CameraDetail extends React.Component<CameraDetailProps, CameraDetailState> {

    private displayedDate: string;
    private displayedCamera: string;
    private footages: Footage[];

    constructor(props: CameraDetailProps)
    {
        super(props);

        this.state = {
            selectedFootage: undefined
        };
    }

    selectFootage(eventClickInfo: any)
    {
        var footage: Footage | undefined = this.footages.find((current: Footage) =>
        {
            return current.id === eventClickInfo.event.id;
        });

        this.setState({ selectedFootage: footage });
    }

    getFootages(info, successCallback, failureCallback)
    {
        var startDate: string = info.start;
        var date: string = moment(startDate).format("YYYYMMDD");

        if (this.props.camera.key === this.displayedCamera && this.displayedDate === date)
        {
            return;
        }

        AjaxService.get<any[]>('api/footages/' + this.props.camera.key + '?date=' + date).then((footagesEvent: Footage[]) =>
        {
            this.footages = footagesEvent;
            this.displayedDate = date;
            this.displayedCamera = this.props.camera.key;

            successCallback(footagesEvent);
        }).catch((ex) =>
        {
            failureCallback(ex);
        });
    }

    render()
    {
        return <div>
            <div className="row pb-3">
                <div className="col">
                    <h2>{this.props.camera.name}</h2>
                </div>
            </div>

            <div className="row pb-3">
                <div className="col-12 col-lg-6">
                    <FullCalendar defaultView="list" eventClick={(eventClickInfo) => this.selectFootage(eventClickInfo)} plugins={[listPlugin]} events={(info, successCallback, failureCallback) => this.getFootages(info, successCallback, failureCallback)}/>
                </div>
                <div className="col-12 col-lg-6">
                    <FootageList footage={this.state.selectedFootage} cameraName={this.props.camera.key} />
                </div>
            </div>
        </div>;
    };
}