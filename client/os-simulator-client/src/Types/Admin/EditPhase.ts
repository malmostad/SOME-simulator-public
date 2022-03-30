import EditScenarioEvent from './EditScenarioEvent';

export default class EditPhase {
    public id: number = -1;
    public start: number = 0;
    public end: number = 0;
    public heading: string = "";
    public scenarioEvents: Array<EditScenarioEvent> = new Array<EditScenarioEvent>();
}