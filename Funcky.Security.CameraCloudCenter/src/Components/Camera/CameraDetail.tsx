import React from "react";
import moment from "moment";

import FullCalendar from '@fullcalendar/react';
import listPlugin from '@fullcalendar/list';

import { Camera } from "../../Models";
import { AjaxService } from "../../Services";

interface CameraDetailProps {
    camera: Camera;
}

interface CameraDetailState {
    footagesEvent: any[];
}

export class CameraDetail extends React.Component<CameraDetailProps, CameraDetailState> {

    loadFootages(date: Date) {
        var momentDate = moment(date);
        AjaxService.get<any[]>('api/footages/' + this.props.camera.key + '?date=' + momentDate.format('YYYYMMDD')).then((footagesEvent) => {
            this.setState({ footagesEvent: footagesEvent });
        });
    }

    constructor(props: CameraDetailProps) {
        super(props);

        this.state = {
            footagesEvent: []
        };
    }

    componentDidMount() {
        this.loadFootages(new Date());
    }

    componentDidUpdate(prevProps) {
        if (this.props.camera.key !== prevProps.camera.key) {

           

            this.loadFootages(new Date());
        }
    }

    render() {
        return <div>
            <div className="row pb-3">
                <div className="col">
                    <h2>{this.props.camera.name}</h2>
                </div>
            </div>

            <div className="row pb-3">
                <div className="col">
                    <FullCalendar defaultView="list" plugins={[listPlugin]} events={this.state.footagesEvent} timezone="UTC"/>
                </div>
            </div>
        </div>;
    };
}
