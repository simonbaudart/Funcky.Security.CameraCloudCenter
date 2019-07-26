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
    displayedDate: Date;

    cameras: Camera[];
    currentCamera?: Camera;

    footages: Footage[];
    currentFootage?: Footage;

    currentSequence?: Footage;
    currentSequenceIndex?: number;
    currentSequenceUrl?: FootageUrl;
}

class CameraPanelComponent extends React.Component<CameraPanelProps, CameraPanelState>
{
    constructor(props: CameraPanelProps)
    {
        super(props);

        this.state = {
            cameras: [],
            footages: [],
            displayedDate: new Date()
        };

        document.addEventListener('keydown', this.handleKeyDown.bind(this));
    }

    componentDidMount()
    {
        this.loadCameras();
    }

    private handleKeyDown(e)
    {
        switch(e.keyCode)
        {
            case 37 : //left
                e.preventDefault();
                this.jumpSequence(-1);
                break;
            case 38 : //up
                this.jumpFootage(-1);
                e.preventDefault();
                break;
            case 39 : //right
                e.preventDefault();
                this.jumpSequence(1);
                break;
            case 40 : //down
                this.jumpFootage(1);
                e.preventDefault();
                break;
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
        this.loadFootages(camera, this.state.displayedDate);
    }

    private jumpDays(jump: number)
    {
        if (!this.state.currentCamera)
        {
            return;
        }

        const newDate = addDays(this.state.displayedDate, jump);
        this.setState({displayedDate: newDate});

        this.loadFootages(this.state.currentCamera, newDate);
    }

    private loadFootages(camera: Camera, date: Date)
    {
        this.setState({footages: [], currentFootage: undefined});

        const formattedDate: string = format(date, 'YYYYMMDD');

        AjaxService.get<any[]>(`api/footages/${camera.key}?date=${formattedDate}`).then((footagesEvent: Footage[]) =>
        {
            let firstFootage: Footage | undefined = undefined;

            if (footagesEvent.length > 0)
            {
                firstFootage = footagesEvent[0];
            }

            this.setState({footages: footagesEvent, currentFootage: firstFootage});

            if (firstFootage)
            {
                this.loadSequence(camera, firstFootage, 0);
            }
        });
    }

    private previousFootage()
    {
        this.jumpFootage(-1);
    }

    private nextFootage()
    {
        this.jumpFootage(1);
    }

    private jumpFootage(jump: number)
    {
        if (!this.state.currentFootage)
        {
            return;
        }

        const currentFootageIndex = this.state.footages.indexOf(this.state.currentFootage);
        const newFootageIndex = currentFootageIndex + jump;
        
        if (newFootageIndex < 0 || newFootageIndex >= this.state.footages.length)
        {
            return;
        }

        const newFootage = this.state.footages[newFootageIndex];
        const newSequenceIndex = jump > 0 ? 0 : newFootage.sequences.length - 1;
        
        this.selectFootage(newFootage, newSequenceIndex);
    }

    private selectFootage(footage: Footage, sequenceIndex: number)
    {
        if (!this.state.currentCamera)
        {
            return;
        }

        this.setState({currentFootage: footage});
        this.loadSequence(this.state.currentCamera, footage, sequenceIndex);
    }

    private jumpSequence(jump: number)
    {
        if (!this.state.currentCamera || !this.state.currentFootage)
        {
            return;
        }

        let currentSequenceIndex = 0;

        if (this.state.currentSequenceIndex)
        {
            currentSequenceIndex = this.state.currentSequenceIndex;
        }

        const newSequenceIndex = currentSequenceIndex + jump;

        if (newSequenceIndex < 0)
        {
            this.previousFootage();
            return;
        }

        if (newSequenceIndex >= this.state.currentFootage.sequences.length)
        {
            this.nextFootage();
            return;
        }

        this.loadSequence(this.state.currentCamera, this.state.currentFootage, newSequenceIndex);
    }

    private loadSequence(camera: Camera, footage: Footage, sequenceIndex: number)
    {
        if (sequenceIndex < 0 || sequenceIndex > footage.sequences.length)
        {
            this.setState({
                currentSequence: undefined,
                currentSequenceIndex: undefined,
                currentSequenceUrl: undefined
            });

            return;
        }

        const currentFootage = footage.sequences[sequenceIndex];
        AjaxService.get('api/footage/' + camera.key + '?id=' + currentFootage.id).then((data: FootageUrl) =>
        {
            this.setState({
                currentSequence: footage.sequences[sequenceIndex],
                currentSequenceIndex: sequenceIndex,
                currentSequenceUrl: data
            });
        });
    }

    private displayCameraDetail()
    {
        if (this.state.currentCamera)
        {
            return <CameraDetail currentCamera={this.state.currentCamera}
                                 displayedDate={this.state.displayedDate}
                                 footages={this.state.footages}

                                 currentFootage={this.state.currentFootage}
                                 
                                 currentSequence={this.state.currentSequence}
                                 currentSequenceUrl={this.state.currentSequenceUrl}
                                 currentSequenceIndex={this.state.currentSequenceIndex}

                                 jumpDays={this.jumpDays.bind(this)}
                                 selectFootage={this.selectFootage.bind(this)}
                                 jumpSequence={this.jumpSequence.bind(this)}
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