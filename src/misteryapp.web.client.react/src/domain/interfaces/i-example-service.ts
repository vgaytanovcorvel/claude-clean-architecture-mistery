import type { Example } from '../models/example'

export interface IExampleService {
  getExamples(): Promise<readonly Example[]>
  getExampleById(id: number): Promise<Example | null>
}
