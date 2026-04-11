// NOTE: Shader automatically converted from Godot Engine 4.5.1.stable.mono's CanvasItemMaterial.

shader_type canvas_item;
render_mode blend_mix;
uniform int particles_anim_h_frames;
uniform int particles_anim_v_frames;
uniform bool particles_anim_loop;

uniform float h: hint_range(0,1) = 1;
uniform float s: hint_range(0,5) = 1;
uniform float v = 1;

// A varying to pass the modulate value to the fragment function
varying vec4 modulate_color;

void vertex() {
	float h_frames = float(particles_anim_h_frames);
	float v_frames = float(particles_anim_v_frames);
	VERTEX.xy /= vec2(h_frames, v_frames);
	float particle_total_frames = float(particles_anim_h_frames * particles_anim_v_frames);
	float particle_frame = floor(INSTANCE_CUSTOM.z * float(particle_total_frames));
	if (!particles_anim_loop) {
		particle_frame = clamp(particle_frame, 0.0, particle_total_frames - 1.0);
	} else {
		particle_frame = mod(particle_frame, particle_total_frames);
	}	UV /= vec2(h_frames, v_frames);
	UV += vec2(mod(particle_frame, h_frames) / h_frames, floor((particle_frame + 0.5) / h_frames) / v_frames);
	modulate_color = COLOR;
}

void fragment() {
    mat3 RGB_to_YIQ = mat3(
        vec3(0.2989,  0.5959,  0.2115),
        vec3(0.5870, -0.2774, -0.5229),
        vec3(0.1140, -0.3216,  0.3114));

    vec4 col = texture(TEXTURE, UV);

    col.rgb = RGB_to_YIQ * col.rgb;
    float hue = 1.0 - h;
    hue = mix(0, 6.283185, hue);

    float sin_hue = sin(hue);
    float cos_hue = cos(hue);
    mat3 hue_shift = mat3(
        vec3(1.0, 0, 0),
        vec3(0, cos_hue, -sin_hue),
        vec3(0, sin_hue, cos_hue));
    col.rgb *= hue_shift;

    mat3 sat_shift = mat3(
        vec3(1.0, 0, 0),
        vec3(0, s, 0),
        vec3(0, 0, s));
    col.rgb = sat_shift * col.rgb;

    col.rgb = mix(vec3(0,0,0), col.rgb, v);

    col.rgb = inverse(RGB_to_YIQ) * col.rgb;
    COLOR = col;
    COLOR *= modulate_color;
}
