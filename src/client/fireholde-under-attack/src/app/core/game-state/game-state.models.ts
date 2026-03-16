export interface SequencedEvent {
  sequenceNumber: number;
  [key: string]: unknown;
}
