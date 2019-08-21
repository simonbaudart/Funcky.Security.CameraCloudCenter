import { /*addDays, */format } from "date-fns";

import AppDispatcher from "./AppDispatcher";
import { EventEmitter } from "events";

import { Camera, Footage, FootageUrl } from "../Models";

import UserStore from "./UserStore";
import Actions from "./Actions";
import Events from "./Events";

import { AjaxService } from "../Services";

interface CameraContent
{
    displayedDate: Date;

    cameras: Camera[];

    currentCamera: Camera | null;

    footages: Footage[] | null;
    currentFootage: Footage | null;

    currentSequence: Footage | null;
    currentSequenceIndex: number;
    currentSequenceUrl: FootageUrl | null;
}

class CameraStore extends EventEmitter
{
    private content: CameraContent;

    constructor()
    {
        super();

        this.content = {
            displayedDate: new Date(),
            cameras: [],
            currentCamera: null,

            footages: null,
            currentFootage: null,

            currentSequence: null,
            currentSequenceIndex: 0,
            currentSequenceUrl: null
        };

        AppDispatcher.register((payload) =>
        {
            switch (payload.actionType)
            {
                case Actions.CameraSelect:
                    const camera: Camera = payload.data;
                    this.selectCamera(camera);
                    break;
            }
        });

        UserStore.addChangeListener(Events.UserChanged, this.loadCameras.bind(this));
    }

    public getContent(): CameraContent
    {
        return this.content;
    }

    public addChangeListener(eventName, callback)
    {
        this.on(eventName, callback);
    }

    public removeChangeListener(eventName, callback)
    {
        this.removeListener(eventName, callback);
    }

    private loadCameras()
    {
        AjaxService.get<Camera[]>("/api/cameras").then((data) =>
        {
            this.content.cameras = data;
            this.content.currentCamera = null;
            this.content.footages = null;
            this.content.currentFootage = null;
            this.content.currentSequence = null;
            this.content.currentSequenceIndex = 0;
            this.content.currentSequenceUrl = null;

            this.emit(Events.CameraListLoaded);
        })
            .catch((code: number) =>
            {
                if (code === 401)
                {
                    // TODO : move it
                    // this.props.context.setRoute(Routes.login);
                }
            });
    }

    private selectCamera(camera: Camera)
    {
        this.content.currentCamera = camera;

        this.loadFootages(camera, this.content.displayedDate);
    }

    private loadFootages(camera: Camera, date: Date)
    {
        this.content.footages = null;
        this.content.currentFootage = null;
        this.content.currentSequence = null;
        this.content.currentSequenceIndex = 0;
        this.content.currentSequenceUrl = null;

        const formattedDate: string = format(date, 'YYYYMMDD');

        AjaxService.get<any[]>(`api/footages/${camera.key}?date=${formattedDate}`).then((footagesEvent: Footage[]) =>
        {
            let firstFootage: Footage | null = null;

            if (footagesEvent.length > 0)
            {
                firstFootage = footagesEvent[0];
            }

            this.content.footages = footagesEvent;
            this.content.currentFootage = firstFootage;

            if (firstFootage)
            {
                this.loadSequence(camera, firstFootage, 0);
            }

            this.emit(Events.CameraFootagesLoaded);
        });
    }

    private loadSequence(camera: Camera, footage: Footage, sequenceIndex: number)
    {
        this.content.currentSequence = null;
        this.content.currentSequenceIndex = 0;
        this.content.currentSequenceUrl = null;

        if (sequenceIndex < 0 || sequenceIndex > footage.sequences.length)
        {
            return;
        }

        const currentFootage = footage.sequences[sequenceIndex];
        AjaxService.get('api/footage/' + camera.key + '?id=' + currentFootage.id).then((data: FootageUrl) =>
        {
            this.content.currentSequence = footage.sequences[sequenceIndex];
            this.content.currentSequenceIndex = sequenceIndex;
            this.content.currentSequenceUrl = data;

            this.emit(Events.CameraSequencesLoaded);
        });
    }
}

export default new CameraStore();