
export default class EditScenarioEvent {
    public id :  number| null = null;
    public phaseId: number| null = null;
    public sender: string = "";
    public text: string = "";
    public timePercent: number  = 0;
    public heading: string = "";
}