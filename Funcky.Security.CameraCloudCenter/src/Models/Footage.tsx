export const FootageTypes =
    {
        video: "recording",
        picture: "snap"
    };

export interface Footage
{
    id: string;
    start: string;
    end: string;
    title: string;
    footageDate: string;
    footageEndDate: string;
    sequences: Footage[];
    type: string;
}