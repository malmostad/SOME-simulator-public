import {MessageFlow} from "@/Types/MessageFlow";

export interface IMessage {
    text: string;
    sender: string;
    messageFlow: MessageFlow;
}