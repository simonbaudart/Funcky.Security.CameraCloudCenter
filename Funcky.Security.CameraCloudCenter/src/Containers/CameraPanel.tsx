import React from "react";
import {CameraDetail, CameraList} from "../Components";
import {Camera, Footage, FootageUrl} from "../Models";
import {AjaxService} from "../Services";
import {Routes} from "../Routing";
import {ContextAwareProps, withContext} from "../Hoc";

import {addDays, format} from "date-fns";

interface CameraPanelProps extends ContextAwareProps
{
}

interface CameraPanelState
{
    cameras: Camera[];

    currentCamera?: Camera;

    displayedDate: Date;

    footages: Footage[];

    selectedFootage?: Footage;

    currentFootage?: Footage;
    currentFootageIndex?: number;
    currentFootageUrl?: FootageUrl;
}

class CameraPanelComponent extends React.Component<CameraPanelProps, CameraPanelState>
{
    constructor(props: CameraPanelProps)
    {
        super(props);

        this.state = {
            cameras: [],
            footages: [],
            displayedDate: new Date(),
            currentFootageIndex: 0
        };
    }

    componentDidMount()
    {
        this.loadCameras();
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
            const nextFootage = this.state.footages[currentFootageIndex - 1];
            this.setState({selectedFootage: nextFootage});
        }
    }

    private nextFootage()
    {
        if (!this.state.selectedFootage)
        {
            return;
        }

        const currentFootageIndex = this.state.footages.indexOf(this.state.selectedFootage);

        if (currentFootageIndex < this.state.footages.length - 1)
        {
            const nextFootage = this.state.footages[currentFootageIndex + 1];
            this.setState({selectedFootage: nextFootage});
        }
    }

    private loadCameras()
    {
        this.setState({cameras: [], currentCamera: undefined});

        AjaxService.get<Camera[]>("/api/cameras").then((data) =>
        {
            this.setState({cameras: data});
        })
            .catch((code: number) =>
            {
                if (code === 401)
                {
                    this.props.context.setRoute(Routes.login);
                }
            });
    }

    private selectCamera(camera: Camera)
    {
        this.setState({currentCamera: camera});
    }

    private selectFootage(footage: Footage)
    {
        this.setState({selectedFootage: footage});
    }

    private loadFootages()
    {
        this.setState({footages: [], selectedFootage: undefined});

        const date: string = format(this.state.displayedDate, 'YYYYMMDD');

        if (!this.state.currentCamera)
        {
            return;
        }

        AjaxService.get<any[]>(`api/footages/${this.state.currentCamera.key}?date=${date}`).then((footagesEvent: Footage[]) =>
        {
            let selectedFootage: Footage | undefined = undefined;

            if (footagesEvent.length > 0)
            {
                selectedFootage = footagesEvent[0];
            }

            this.setState({footages: footagesEvent, selectedFootage: selectedFootage});
        });
    }

    private addDays(jump: number)
    {
        const newDate = addDays(this.state.displayedDate, jump);
        this.setState({displayedDate: newDate});
        this.loadFootages();
    }

    private moveFootage(jump: number)
    {
        this.navigateFootage(jump);
    }

    private navigateFootage(jump: number)
    {
        if (!this.state.selectedFootage || !this.state.currentFootageIndex)
        {
            return;
        }
        
        const index = this.state.currentFootageIndex + jump;

        if (index < 0)
        {
            this.previousFootage();
            return;
        }

        if (index >= this.state.selectedFootage.sequences.length)
        {
            this.nextFootage();
            return;
        }

        this.setState({currentFootageIndex: index});
        this.loadCurrentFootage();
    }

    private loadCurrentFootage()
    {
        if (!this.state.selectedFootage || !this.state.currentCamera || !this.state.currentFootageIndex)
        {
            return;
        }
        
        const currentFootage = this.state.selectedFootage.sequences[this.state.currentFootageIndex];
        AjaxService.get('api/footage/' + this.state.currentCamera.key + '?id=' + currentFootage.id).then((data: FootageUrl) =>
        {
            this.setState({
                currentFootageUrl: data
            });
        });
    }

    private displayCameraDetail()
    {
        if (this.state.currentCamera)
        {
            return <CameraDetail camera={this.state.currentCamera}
                                 displayedDate={this.state.displayedDate}
                                 footages={this.state.footages}
                                 previousFootage={this.previousFootage.bind(this)}
                                 nextFootage={this.nextFootage.bind(this)}
                                 addDays={this.addDays.bind(this)}
                                 selectFootage={this.selectFootage.bind(this)}
                                 moveFootage={this.moveFootage.bind(this)}
                                 currentFootage={this.state.currentFootage}
                                 currentFootageUrl={this.state.currentFootageUrl}
                                 currentFootageIndex={this.state.currentFootageIndex}
            />;
        }

        return <></>;
    }

    render()
    {
        return <>
            <div className="row pb-3">
                <div className="col-12 col-lg-3">
                    <CameraList cameras={this.state.cameras} selectCamera={this.selectCamera.bind(this)}/>
                </div>
                <div className="col-12 col-lg-9">
                    {this.displayCameraDetail()}
                </div>
            </div>
        </>;
    }
}

export const CameraPanel = withContext<CameraPanelProps>(CameraPanelComponent);