import React from "react";
import moment from "moment";

import FullCalendar from '@fullcalendar/react';
import listPlugin from '@fullcalendar/list';

import { Camera } from "../../Models";
import { AjaxService } from "../../Services";

interface CameraDetailProps
{
    camera: Camera;
}

interface CameraDetailState
{
    event: any;
}

export class CameraDetail extends React.Component<CameraDetailProps, CameraDetailState> {

    private displayedDate: string;
    private displayedCamera: string;

    constructor(props: CameraDetailProps)
    {
        super(props);

        this.state = {
            event: undefined
        };
    }

    selectFootage(eventClickInfo: any)
    {
        console.log(eventClickInfo);

        this.setState({ event: eventClickInfo });
    }

    getFootages(info, successCallback, failureCallback)
    {
        var startDate: string = info.start;
        var date: string = moment(startDate).format("YYYYMMDD");

        if (this.props.camera.key === this.displayedCamera && this.displayedDate === date)
        {
            return;
        }

        AjaxService.get<any[]>('api/footages/' + this.props.camera.key + '?date=' + date).then((footagesEvent) =>
        {
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
                <div className="col-6">
                    <FullCalendar defaultView="list" eventClick={(eventClickInfo) => this.selectFootage(eventClickInfo)} plugins={[listPlugin]} events={(info, successCallback, failureCallback) => this.getFootages(info, successCallback, failureCallback)} timezone="UTC" />
                </div>
            </div>
        </div>;
    };
}
