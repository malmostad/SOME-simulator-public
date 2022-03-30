import {MessageFlow} from "@/Types/MessageFlow";

export default class Comment {
    public id :  number| null = null;
    public messageFlow:MessageFlow = 0;
    public phases: Array<number> = [] ;
    public sender: string = "";
    public text: string = "";
}