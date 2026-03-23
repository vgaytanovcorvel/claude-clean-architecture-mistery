import type { Example } from '../models/example'

export interface IExampleRepository {
  exampleFindAll(): Promise<readonly Example[]>
  exampleSingleById(id: number): Promise<Example>
  exampleSingleOrDefaultById(id: number): Promise<Example | null>
}
