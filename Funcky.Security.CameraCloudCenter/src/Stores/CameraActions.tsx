import Actions from "./Actions";
import AppDispatcher from "./AppDispatcher";

import {Camera, Footage} from "Models";

class CameraActions
{
    public selectCamera(camera: Camera)
    {
        AppDispatcher.dispatch({
            actionType: Actions.CameraSelect,
            data: camera
        });
    }

    public nextDate()
    {
        AppDispatcher.dispatch({
            actionType: Actions.DateJump,
            data: 1
        });
    }

    public previousDate()
    {
        AppDispatcher.dispatch({
            actionType: Actions.DateJump,
            data: -1
        });
    }

    public selectFootage(footage: Footage)
    {
        AppDispatcher.dispatch({
            actionType: Actions.FootageSelect,
            data: footage
        });
    }

    public nextSequence()
    {
        AppDispatcher.dispatch({
            actionType: Actions.SequenceJump,
            data: 1
        });
    }

    public previousSequence()
    {
        AppDispatcher.dispatch({
            actionType: Actions.SequenceJump,
            data: -1
        });
    }
}

export default new CameraActions()