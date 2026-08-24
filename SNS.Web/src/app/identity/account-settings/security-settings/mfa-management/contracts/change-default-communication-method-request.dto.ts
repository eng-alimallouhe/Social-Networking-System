import { CommunicationMethod } from "../../../../shared/contracts/communication-method.enum";

export interface ChangeDefaultCommunicationMethodCommand {
    newCommunicationMethod: CommunicationMethod;
}
