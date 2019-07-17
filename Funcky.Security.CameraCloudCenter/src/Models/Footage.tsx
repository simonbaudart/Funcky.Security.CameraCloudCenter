export interface Footage
{
    id: string;
    start: string;
    end: string;
    footageDate: string;
    footageEndDate: string;
    sequences: Footage[];
}