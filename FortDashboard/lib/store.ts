type State = { [key: string]: any }

type Getter = { [key: string]: () => any }

type Action = { [key: string]: Function }

class Store {
  state: State = {}
  getters: Getter = {}
  actions: Action = {}

  constructor() {
    this.state = {}
    this.getters = {}
    this.actions = {}
  }

  defineState(state: State) {
    this.state = { ...this.state, ...state }
  }

  defineGetters(getters: Getter) {
    this.getters = { ...this.getters, ...getters }
  }

  defineActions(actions: Action) {
    this.actions = { ...this.actions, ...actions }
  }

  getState(key: string) {
    return this.state[key]
  }

  setState(key: string, value: any) {
    this.state[key] = value
  }

  dispatch(action: string, ...args: any[]) {
    if (this.actions[action]) {
      return this.actions[action](...args)
    }
  }

  getGetter(key: string) {
    return this.getters[key] ? this.getters[key]() : null
  }
}

const store = new Store()

export default store
