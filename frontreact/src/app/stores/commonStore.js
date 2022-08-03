import { makeAutoObservable, reaction } from 'mobx';
import UserStore from './userStore';

export default class CommonStore {
	error = null;
	token = window.localStorage.getItem('jwt');
	appLoaded = false;

	constructor() {
		makeAutoObservable(this);

		console.log('%c *** constructor CommonStore ***', 'color:green', this.token);

		reaction(
			() => this.token,
			token => {
				if (token) {
					window.localStorage.setItem('jwt', token);
				} else {
					window.localStorage.removeItem('jwt');
				}
			}
		);
		this.noviUserstore = new UserStore();
		this.init();
	}

	init = () => {
		if (this.token) {
			this.noviUserstore.getUser().finally(() => this.setAppLoaded());
		} else {
			this.setAppLoaded();
		}
		this.noviUserstore.pokus();
	};

	setServerError = error => {
		this.error = error;
	};

	setToken = token => {
		this.token = token;
	};

	setAppLoaded = () => {
		this.appLoaded = true;
	};
}
