/**
 * Creates a button of the specified type, with the given classes and text
 *
 * @param button.type The type of button cancel, back, or next
 * @param button.classes Classes to apply to the button
 * @param button.text The text for the button
 * @param button.action The action to call
 */
export function makeButton(button) {
    const { classes, disabled, label, secondary, type, text } = button;
    const builtInButtonTypes = ['back', 'cancel', 'next'];
    if (!type) {
        return button;
    }
    if (builtInButtonTypes.indexOf(type) === -1) {
        throw new Error(`'type' property must be one of 'back', 'cancel', or 'next'`);
    }
    return {
        action: this[type].bind(this),
        classes,
        disabled,
        label,
        secondary,
        text
    };
}
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiYnV0dG9ucy5qcyIsInNvdXJjZVJvb3QiOiIiLCJzb3VyY2VzIjpbIi4uLy4uLy4uLy4uLy4uL3Byb2plY3RzL3NoZXBoZXJkL3NyYy9saWIvdXRpbHMvYnV0dG9ucy50cyJdLCJuYW1lcyI6W10sIm1hcHBpbmdzIjoiQUFBQTs7Ozs7OztHQU9HO0FBQ0gsTUFBTSxVQUFVLFVBQVUsQ0FBQyxNQUFNO0lBQy9CLE1BQU0sRUFBRSxPQUFPLEVBQUUsUUFBUSxFQUFFLEtBQUssRUFBRSxTQUFTLEVBQUUsSUFBSSxFQUFFLElBQUksRUFBRSxHQUFHLE1BQU0sQ0FBQztJQUNuRSxNQUFNLGtCQUFrQixHQUFHLENBQUMsTUFBTSxFQUFFLFFBQVEsRUFBRSxNQUFNLENBQUMsQ0FBQztJQUV0RCxJQUFJLENBQUMsSUFBSSxFQUFFO1FBQ1QsT0FBTyxNQUFNLENBQUM7S0FDZjtJQUVELElBQUksa0JBQWtCLENBQUMsT0FBTyxDQUFDLElBQUksQ0FBQyxLQUFLLENBQUMsQ0FBQyxFQUFFO1FBQzNDLE1BQU0sSUFBSSxLQUFLLENBQUMsNERBQTRELENBQUMsQ0FBQztLQUMvRTtJQUVELE9BQU87UUFDTCxNQUFNLEVBQUUsSUFBSSxDQUFDLElBQUksQ0FBQyxDQUFDLElBQUksQ0FBQyxJQUFJLENBQUM7UUFDN0IsT0FBTztRQUNQLFFBQVE7UUFDUixLQUFLO1FBQ0wsU0FBUztRQUNULElBQUk7S0FDTCxDQUFDO0FBQ0osQ0FBQyIsInNvdXJjZXNDb250ZW50IjpbIi8qKlxuICogQ3JlYXRlcyBhIGJ1dHRvbiBvZiB0aGUgc3BlY2lmaWVkIHR5cGUsIHdpdGggdGhlIGdpdmVuIGNsYXNzZXMgYW5kIHRleHRcbiAqXG4gKiBAcGFyYW0gYnV0dG9uLnR5cGUgVGhlIHR5cGUgb2YgYnV0dG9uIGNhbmNlbCwgYmFjaywgb3IgbmV4dFxuICogQHBhcmFtIGJ1dHRvbi5jbGFzc2VzIENsYXNzZXMgdG8gYXBwbHkgdG8gdGhlIGJ1dHRvblxuICogQHBhcmFtIGJ1dHRvbi50ZXh0IFRoZSB0ZXh0IGZvciB0aGUgYnV0dG9uXG4gKiBAcGFyYW0gYnV0dG9uLmFjdGlvbiBUaGUgYWN0aW9uIHRvIGNhbGxcbiAqL1xuZXhwb3J0IGZ1bmN0aW9uIG1ha2VCdXR0b24oYnV0dG9uKSB7XG4gIGNvbnN0IHsgY2xhc3NlcywgZGlzYWJsZWQsIGxhYmVsLCBzZWNvbmRhcnksIHR5cGUsIHRleHQgfSA9IGJ1dHRvbjtcbiAgY29uc3QgYnVpbHRJbkJ1dHRvblR5cGVzID0gWydiYWNrJywgJ2NhbmNlbCcsICduZXh0J107XG5cbiAgaWYgKCF0eXBlKSB7XG4gICAgcmV0dXJuIGJ1dHRvbjtcbiAgfVxuXG4gIGlmIChidWlsdEluQnV0dG9uVHlwZXMuaW5kZXhPZih0eXBlKSA9PT0gLTEpIHtcbiAgICB0aHJvdyBuZXcgRXJyb3IoYCd0eXBlJyBwcm9wZXJ0eSBtdXN0IGJlIG9uZSBvZiAnYmFjaycsICdjYW5jZWwnLCBvciAnbmV4dCdgKTtcbiAgfVxuXG4gIHJldHVybiB7XG4gICAgYWN0aW9uOiB0aGlzW3R5cGVdLmJpbmQodGhpcyksXG4gICAgY2xhc3NlcyxcbiAgICBkaXNhYmxlZCxcbiAgICBsYWJlbCxcbiAgICBzZWNvbmRhcnksXG4gICAgdGV4dFxuICB9O1xufVxuIl19