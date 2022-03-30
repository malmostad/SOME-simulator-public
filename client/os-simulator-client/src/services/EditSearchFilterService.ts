
import {MessageFlow} from "@/Types/MessageFlow";
import {IMessage} from "@/Types/Admin/IMessage";

export class EditSearchFilterData {
    public text: string = "";
    public messageFlow: MessageFlow = 0;
}

export class EditSearchFilterService {
    public filter(searchFilterData: EditSearchFilterData, data: IMessage[]): IMessage[] {
        
        if (data == null) return [];
        let result : IMessage[] = [...data];
        
        if(searchFilterData.messageFlow > 0) {
            result = result.filter((d) => {
                return ((d.messageFlow & searchFilterData.messageFlow) == searchFilterData.messageFlow);
            });
        }

        if(searchFilterData.text.length > 0) {
            result = result.filter((d) => {
                
                
                return (d.sender.toLowerCase().indexOf(searchFilterData.text.toLowerCase()) >= 0 || d.text.toLowerCase().indexOf(searchFilterData.text.toLowerCase()) >= 0);
            });
        }
        
        return [...result];
        
    }
}