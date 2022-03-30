import {MessageFlow} from "@/Types/MessageFlow";

export default class EditComment {
    constructor(scenarioId:number) {
        this.scenarioId = scenarioId;
    }
    public scenarioId: number;
    public id :  number| null = null;
    public messageFlow:MessageFlow = 0;
    public phases: Array<number> = [] ;
    public sender: string = "";
    public text: string = "";
    public props: number = 0;
}