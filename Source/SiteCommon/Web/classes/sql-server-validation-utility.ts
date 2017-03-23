export class SqlServerValidationUtility {
     static invalidUsernames: string[] = [
        'admin',
        'administrator',
        'dbmanager',
        'dbo',
        'guest',
        'loginmanager',
        'public',
        'root',
        'sa'
    ];

     static invalidServerNames: string[] = [];

     static validateAzureSQLCreate(server: string, username: string, password: string, password2:string): string {
         let passwordError: string = SqlServerValidationUtility.validatePassword(password, password2, 8);
         let servernameError: string = SqlServerValidationUtility.validateUsername(server, SqlServerValidationUtility.invalidServerNames, 'Server name');
         let usernameError: string = SqlServerValidationUtility.validateUsername(username, SqlServerValidationUtility.invalidUsernames, 'Username');

         return passwordError || servernameError || usernameError || '';
     }

     static validatePassword(pwd: string, pwd2: string, length: number): string {
         let passwordError: string = '';
         if (pwd !== pwd2) {
             passwordError = 'Passwords do not match.';
         } else if (length && pwd.length < length) {
             passwordError = 'Password must be at least eight characters long.';
         } else if ((/\s/g).test(pwd)) {
             passwordError = 'Password should not contain spaces.';
         } else if (!(/[A-Z]/).test(pwd) || (/^[a-zA-Z0-9]*$/).test(pwd)) {
             passwordError = 'Password must contain at least one uppercase character and at least one special character.';
         }
         return passwordError;
     }

     static validateUsername(username: string, invalidUsernames: string[], usernameText: string): string {
         let usernameError: string = '';
         if ((/\s/g).test(username)) {
             usernameError = `${usernameText} should not contain spaces.`;
         } else if (username.length > 63) {
             usernameError = `${usernameText} must not be longer than 63 characters.`;
         } else if (invalidUsernames.indexOf(username.toLowerCase()) > -1) {
             usernameError = `${usernameText} cannot be a reserved SQL system name.`;
         } else if (!(/^[a-zA-Z0-9\-]+$/).test(username)) {
             usernameError = `${usernameText} must only contain alphanumeric characters or hyphens.`;
         } else if (username[0] === '-' || username[username.length - 1] === '-') {
             usernameError = `${usernameText} must not start or end with a hyphen.`;
         }
         return usernameError;
     }
}