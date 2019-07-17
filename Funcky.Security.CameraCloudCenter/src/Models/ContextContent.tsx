import { Camera } from "./";

export interface ContextContent
{
    route: string;
    setRoute: (route: string) => void;
    updateContext: (context: ContextContent) => void;
    currentCamera?: Camera;
}