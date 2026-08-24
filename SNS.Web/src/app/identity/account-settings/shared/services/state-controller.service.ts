import { Injectable, signal } from "@angular/core";

@Injectable({
    providedIn: "root"
})
export class StateControllerService {
    private _actionState = signal<ActionState>({
        state: State.Ready,
        messageKey: ""
    });

    private _isBusy = signal<boolean>(false);

    public actionState = this._actionState.asReadonly();

    public isBusy = this._isBusy.asReadonly();

    public start(stateMessageKey: string): void {
        this._isBusy.set(true);
        this._actionState.set({
            state: State.Started,
            messageKey: stateMessageKey
        });
    }

    public run(): void {
        this._actionState.update((currentState) => ({
            state: State.Running,
            messageKey: currentState.messageKey
        }));
    }

    public stop(): void {
        this._isBusy.set(false);
        this._actionState.update((currentState) => ({
            state: State.Stoped,
            messageKey: currentState.messageKey
        }));
    }

    public ready(stateMessageKey: string): void {
        this._isBusy.set(false);
        this._actionState.set({
            state: State.Ready,
            messageKey: stateMessageKey
        });
    }
}

export interface ActionState {
    state: State;
    messageKey: string;
}

export enum State {
    Started = "started",
    Running = "running",
    Stoped = "stoped",
    Ready = "ready"
}