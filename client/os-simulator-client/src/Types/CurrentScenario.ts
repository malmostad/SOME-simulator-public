import { Scenario } from '../Types/Scenario';
import ScenarioEvent from '../Types/ScenarioEvent';
import Phase from '../Types/Phase';

export default interface CurrentScenario {
    scenario: Scenario | null;
    events: ScenarioEvent[];
    phases: Phase[];
}
