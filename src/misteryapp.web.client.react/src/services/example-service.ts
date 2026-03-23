import type { IExampleService } from '../domain/interfaces/i-example-service'
import type { IExampleRepository } from '../domain/interfaces/i-example-repository'
import type { Example } from '../domain/models/example'

export class ExampleService implements IExampleService {
  constructor(private readonly exampleRepo: IExampleRepository) {}

  getExamples(): Promise<readonly Example[]> {
    return this.exampleRepo.exampleFindAll()
  }

  getExampleById(id: number): Promise<Example | null> {
    return this.exampleRepo.exampleSingleOrDefaultById(id)
  }
}
