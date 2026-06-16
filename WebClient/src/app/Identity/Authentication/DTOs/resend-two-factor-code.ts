import { CodeSendMethod } from "../../Shared/enums/code-send-method";

export interface ResendTwoFactorCodeDto {
    userId: string;
    codeSendMethod: CodeSendMethod;
}