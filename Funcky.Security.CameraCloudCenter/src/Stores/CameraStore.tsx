import { addDays, format } from "date-fns";

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
                case Actions.DateJump:
                    const days: number = payload.data;
                    this.jumpDays(days);
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
        });
    }

    private selectCamera(camera: Camera)
    {
        this.content.currentCamera = camera;

        this.loadFootages();
    }

    private loadFootages()
    {
        this.content.footages = null;
        this.content.currentFootage = null;
        this.content.currentSequence = null;
        this.content.currentSequenceIndex = 0;
        this.content.currentSequenceUrl = null;

        if (!this.content.currentCamera)
        {
            return;
        }

        const formattedDate: string = format(this.content.displayedDate, 'YYYYMMDD');

        AjaxService.get<any[]>(`api/footages/${this.content.currentCamera.key}?date=${formattedDate}`).then((footagesEvent: Footage[]) =>
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
                this.loadSequence();
            }

            this.emit(Events.CameraFootagesLoaded);
        });
    }

    private loadSequence()
    {
        this.content.currentSequence = null;
        this.content.currentSequenceIndex = 0;
        this.content.currentSequenceUrl = null;

        if (!this.content.currentCamera || !this.content.currentFootage)
        {
            return;
        }

        if (this.content.currentSequenceIndex < 0 || this.content.currentSequenceIndex > this.content.currentFootage.sequences.length)
        {
            return;
        }

        const currentFootage = this.content.currentFootage.sequences[this.content.currentSequenceIndex];
        AjaxService.get('api/footage/' + this.content.currentCamera.key + '?id=' + currentFootage.id).then((data: FootageUrl) =>
        {
            if (!this.content.currentCamera || !this.content.currentFootage)
            {
                return;
            }

            this.content.currentSequence = this.content.currentFootage.sequences[this.content.currentSequenceIndex];
            this.content.currentSequenceIndex = this.content.currentSequenceIndex;
            this.content.currentSequenceUrl = data;

            this.emit(Events.CameraSequencesLoaded);
        });
    }

    private jumpDays(jump: number)
    {
        if (!this.content.currentCamera)
        {
            return;
        }

        this.content.displayedDate = addDays(this.content.displayedDate, jump);

        this.loadFootages();
    }
}

export default new CameraStore();