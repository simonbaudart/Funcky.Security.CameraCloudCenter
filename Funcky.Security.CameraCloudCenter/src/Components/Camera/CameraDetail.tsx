import React from "react";
// ReSharper disable once UnusedLocalImport
import format from 'date-fns/format';
import addDays from 'date-fns/add_days';

import {Camera, Footage} from "../../Models";
import {AjaxService} from "../../Services";

import {FootageList} from "../";

interface CameraDetailProps
{
    camera: Camera;
}

interface CameraDetailState
{
    selectedFootage: Footage | undefined;
    displayedDate: Date;
    footages: Footage[];
}

export class CameraDetail extends React.Component<CameraDetailProps, CameraDetailState>
{
    constructor(props: CameraDetailProps)
    {
        super(props);

        this.state = {
            selectedFootage: undefined,
            displayedDate: new Date(),
            footages: []
        };
    }

    componentDidMount(): void
    {
        this.loadFootages(this.props.camera);
    }

    componentWillReceiveProps(nextProps: CameraDetailProps)
    {
        if (this.props.camera !== nextProps.camera)
        {
            this.setState({
                selectedFootage: undefined,
                displayedDate: new Date(),
                footages: []
            });

            this.loadFootages(nextProps.camera);
        }
    }

    private loadFootages(camera: Camera)
    {
        const date: string = format(this.state.displayedDate, 'YYYYMMDD');

        AjaxService.get<any[]>(`api/footages/${camera.key}?date=${date}`).then((footagesEvent: Footage[]) =>
        {
            let selectedFootage: Footage | undefined = undefined;

            if (footagesEvent.length > 0)
            {
                selectedFootage = footagesEvent[0];
            }

            this.setState({footages: footagesEvent, selectedFootage: selectedFootage});
        });
    }

    private addDays(e: React.MouseEvent<HTMLButtonElement>, amount: number)
    {
        e.preventDefault();

        const newDate = addDays(this.state.displayedDate, amount);
        this.setState({displayedDate: newDate});
        this.loadFootages(this.props.camera);
    }

    private selectFootage(e: React.MouseEvent<HTMLButtonElement>, footage: Footage)
    {
        e.preventDefault();

        this.setState({selectedFootage: footage});
    }

    private nextFootage()
    {
        if (!this.state.selectedFootage)
        {
            return;
        }
        
        const currentFootageIndex = this.state.footages.indexOf(this.state.selectedFootage);
        
        if (currentFootageIndex < this.state.footages.length -1)
        {
            const nextFootage = this.state.footages[currentFootageIndex + 1];
            this.setState({selectedFootage: nextFootage});    
        }
    }

    private previousFootage()
    {
        if (!this.state.selectedFootage)
        {
            return;
        }

        const currentFootageIndex = this.state.footages.indexOf(this.state.selectedFootage);

        if (currentFootageIndex >= 1)
        {
            const nextFootage = this.state.footages[currentFootageIndex -1];
            this.setState({selectedFootage: nextFootage});
        }
    }

    public render()
    {
        let footageList = <></>;

        if (this.state.selectedFootage)
        {
            footageList = <FootageList footage={this.state.selectedFootage} cameraName={this.props.camera.key}
                                       previousFootage={() => this.previousFootage()}
                                       nextFootage={() => this.nextFootage()}/>;
        }

        return <div>
            <div className="row">
                <div className="col">
                    <h2>{this.props.camera.name}</h2>
                </div>
            </div>

            <div className="row">
                <div className="col-12">
                    <h3>
                        {format(this.state.displayedDate, 'DD/MM/YYYY')}
                        <button type="button" className="btn btn-secondary ml-2"
                                onClick={(e) => this.addDays(e, -1)}>&lt;</button>
                        <button type="button" className="btn btn-primary"
                                onClick={(e) => this.addDays(e, 1)}>&gt;</button>
                    </h3>

                </div>
            </div>

            <div className="row">
                <div className="col-12 col-lg-6 m-auto">
                    {footageList}
                </div>
            </div>

            <div className="row pb-3">
                <div className="list-group">
                    {
                        this.state.footages.map((footage) =>
                        {
                            return <button key={footage.id} type="button" className={
                                "list-group-item list-group-item-action " +
                                (this.state.selectedFootage && this.state.selectedFootage === footage ? "active" : "")
                            } onClick={(e) => this.selectFootage(e, footage)}>
                                {footage.title}
                            </button>
                        })
                    }
                </div>
            </div>
        </div>;
    };
}