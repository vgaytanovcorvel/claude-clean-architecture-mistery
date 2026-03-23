import type { IExampleRepository } from '../domain/interfaces/i-example-repository'
import type { Example } from '../domain/models/example'
import { NotFoundException } from '../domain/errors'
import type { ApiClient } from '../core/api-client'

interface ExampleDto {
  id: number
  name: string
  description: string
  isActive: boolean
  createdAt: string
}

export class HttpExampleRepository implements IExampleRepository {
  constructor(private readonly http: ApiClient) {}

  async exampleFindAll(): Promise<readonly Example[]> {
    const dtos = await this.http.get<ExampleDto[]>('/api/examples')
    return dtos.map((dto) => this.mapToDomain(dto))
  }

  async exampleSingleById(id: number): Promise<Example> {
    const example = await this.exampleSingleOrDefaultById(id)
    if (!example) throw new NotFoundException(`Example not found (ExampleId: ${id})`)
    return example
  }

  async exampleSingleOrDefaultById(id: number): Promise<Example | null> {
    const dto = await this.http.getOrNull<ExampleDto>(`/api/examples/${id}`)
    return dto ? this.mapToDomain(dto) : null
  }

  private mapToDomain(dto: ExampleDto): Example {
    return {
      id: dto.id,
      name: dto.name,
      description: dto.description,
      isActive: dto.isActive,
      createdAt: new Date(dto.createdAt),
    }
  }
}
